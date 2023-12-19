
using DeviceManager.Enums;
using DeviceManager.Migrations;
using DeviceManager.Pages.Provider;
using DeviceManager.Repository;
using DeviceManager.Shared;
using ElectronNET.API.Entities;
using Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MudBlazor;
using SocketIOClient.Messages;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;

namespace DeviceManager.Pages.Patient
{
    public class PatientDetailsBase : Microsoft.AspNetCore.Components.ComponentBase
    {
        public List<PatientEntity>? PatientDto { get; set; }
        protected string searchString1 = "";
        protected PatientEntity selectedItem1 = null;
        public HashSet<PatientEntity> selectedItems = new HashSet<PatientEntity>();
        protected MudTable<PatientEntity> mudTable;
        DialogOptions disableBackdropClick = new DialogOptions() { DisableBackdropClick = true };
        protected string headerValue = "Patient";
        IList<IBrowserFile> files = new List<IBrowserFile>();
        protected bool pageLoaded { get; set; } = false;
        [Inject]
        protected IDialogService Dialog { get; set; }
        [Inject]
        protected IPatientRepository patientRepository { get; set; }
        [Inject]
        protected IProviderRepository providerRepository { get; set; }
        [Inject]
		NavigationManager navigationManager { get; set; }

        [Parameter]
        public string? providerId { get; set; }
		[Inject] public StateDetails? stateDetails { get; set; }
		public bool FilterFunc1(PatientEntity element) => FilterFunc(element, searchString1);

        private bool FilterFunc(PatientEntity element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;

            if (element.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        protected override async Task OnInitializedAsync()
        {
            PatientDto = new List<PatientEntity>();
            if (string.IsNullOrEmpty(providerId) || !Guid.TryParse(providerId, out Guid guidProviderId) )
            {
                headerValue = "";
                PatientDto.AddRange(await patientRepository.GetAllPatient());
            }
            else
            {
                var provider = await providerRepository.GetProviderById(guidProviderId);
                headerValue = $"Provider ({provider?.LegalName})";
                PatientDto.AddRange(await patientRepository.GetPatientByProviderId(guidProviderId));
            }
            await base.OnInitializedAsync();
            pageLoaded = true;
        }

        protected void OnEditClick(Guid id)
        {
            var b = mudTable.SelectedItem;
            var a = selectedItem1;
			navigationManager.NavigateTo($"/EditPatient/{id}");
		}

        protected void OnClaimClick(Guid PatientId)
        {
            navigationManager.NavigateTo($"/claim/{PatientId}");
        }
        protected void OnDeleteClick(Guid id)
        {
            var parameters = new DialogParameters<DeleteDialog>();
            parameters.Add(x => x.ContentText, "Do you really want to delete these patient? This process cannot be undone. It is a cascading delete");
            parameters.Add(x => x.ButtonText, "Delete");
            parameters.Add(x => x.Color, MudBlazor.Color.Error);
            parameters.Add(x => x.entityTypeEnum, EntityTypeEnum.Patient);
            parameters.Add(x => x.Id, id.ToString()); 

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, DisableBackdropClick = true };

            Dialog.Show<DeleteDialog>("Delete", parameters, options);
        }

        protected async void UploadFiles2(IBrowserFile file)
        {
             string fileContent = "";
             files.Add(file);
            
            long maxsize = 512000;

            var buffer = new byte[file.Size];
            await file.OpenReadStream(maxsize).ReadAsync(buffer);
            fileContent = System.Text.Encoding.UTF8.GetString(buffer);
            await ParsePatientAndProviderText(fileContent);
            //TODO upload the files to the server
        }

        private async Task ParsePatientAndProviderText(string result)
        {
            string line= string.Empty;
            string[] lines = result.Split("\r\n");
            PatientEntity patientEntity = new PatientEntity();
            ProviderEntity providerEntity = new ProviderEntity();
            string errorMessage = string.Empty;
            try
            {
                if (lines != null && lines.Length >= 1)
                {
                    var providerName = lines[0];

                    #region Rrovider Details
                    if (string.IsNullOrEmpty(providerName))
                    {
                        errorMessage = "Provider Name is required";
                    }
                    providerEntity.LegalName = providerName;

                    var providerDetails = lines[1];
                    if (string.IsNullOrEmpty(providerDetails))
                    {
                        errorMessage = "Provider Details is required";
                    }
                    var providerDetailsAfterCommaSplit = providerDetails.Split(",");
                    if(providerDetailsAfterCommaSplit != null && providerDetailsAfterCommaSplit.Length > 0)
                    {
                        var providerAddress = providerDetailsAfterCommaSplit[0];
                        if (string.IsNullOrEmpty(providerAddress))
                        {
                            errorMessage = "Provider Address is required";
                        }
                        providerAddress.Trim();
                        var city= providerDetailsAfterCommaSplit[1];
                        if (string.IsNullOrEmpty(city))
                        {
                            errorMessage = "Provider City is required";
                        }
                        

                        var StateAndZip = providerDetailsAfterCommaSplit[2].Substring(0,9);
                        if (string.IsNullOrEmpty(StateAndZip))
                        {
                            errorMessage = "Provider State and Zip is required";
                        }
                        StateAndZip = StateAndZip.Trim().Replace(" ","");

                        var state = StateAndZip.Substring(0,2);
                        var zipCode = StateAndZip.Substring(2);
                        if (!int.TryParse(zipCode, out int zip))
                        {
                            errorMessage = "Provider  Zip is required";

                        }

                        var telephoneSplit = providerDetailsAfterCommaSplit[2].Split("Tel:");
                        if(telephoneSplit == null || telephoneSplit.Length <= 1)
                        {
                            errorMessage = "Provider  phone number is required";
                        }
                        var number = telephoneSplit[1].Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                        var phone = number.Substring(0, 10);
                        if(!long.TryParse(phone, out long phoneNumber))
                        {
                            errorMessage = "Provider  phone number is required";
                        }
                        providerEntity.State = state.Trim();
                        providerEntity.City = city.Trim();
                        providerEntity.PhoneNumber = phoneNumber.ToString();
                        providerEntity.Address = providerAddress.Trim();
                        providerEntity.LegalName = providerName.Trim();
                        providerEntity.ZipCode = zip;

                    }
                    else
                    {
                        errorMessage = "Provider Name and Details is required";
                    }

                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        ShowPDFConfirmationDialog(true, $"Error while parsing with error as {errorMessage}");
                        return;
                    }

                    #endregion
                    errorMessage = string.Empty;
                    foreach (string sline in lines)
                    {
                        line = sline;
                        if (line.StartsWith("LAST NAME") && line.Contains("FIRST NAME"))
                        {
                            var lastName = line.Substring("LAST NAME".Length, line.IndexOf("FIRST NAME") - "FIRST NAME".Length);
                            if (!string.IsNullOrEmpty(lastName))
                            {
                                lastName = lastName.Trim();

                            }
                            if (line.Contains("MI") && line.LastIndexOf("MI") > line.IndexOf("FIRST NAME"))
                            {
                                var firstName = line.Substring(line.IndexOf("FIRST NAME") + "FIRST NAME".Length, line.LastIndexOf("MI") - line.IndexOf("FIRST NAME") - "FIRST NAME".Length);
                                var middleName = line.Substring(line.LastIndexOf("MI") + 2, line.Length - line.LastIndexOf("MI") - 2);
                                if (string.IsNullOrWhiteSpace(firstName))
                                {
                                    errorMessage = "Patient First name is required";
                                }
                                firstName = firstName.Trim();
                                if (!string.IsNullOrEmpty(middleName))
                                {
                                    firstName = firstName + " " + middleName.Trim();
                                }

                                if (!string.IsNullOrEmpty(lastName))
                                {
                                    patientEntity.FullName = lastName + "," + firstName;
                                }
                                else
                                {
                                    patientEntity.FullName = "," + firstName;
                                }

                            }

                        }

                        else if (line.StartsWith("ADDRESS") && line.Contains("APT / SUITE"))
                        {
                            var address = line.Substring(9, (line.IndexOf("APT / SUITE") - 9));
                            if (string.IsNullOrEmpty(address))
                            {
                                errorMessage = "Patient Address is required";
                            }
                            address = address.Trim();
                            patientEntity.Address = address;
                        }

                        else if (line.StartsWith("CITY"))
                        {
                            var city = line.Replace("CITY", "");
                            if (string.IsNullOrEmpty(city))
                            {
                                // throw error
                            }
                            city = city.Trim();
                            patientEntity.City = city;
                        }

                        else if (line.StartsWith("STATE"))
                        {
                            var state = line.Substring(6, 2);
                            if (string.IsNullOrEmpty(state))
                            {
                                errorMessage = "Patient state is required";
                            }
                            patientEntity.state = state;
                            var ZipCode = (line.Substring(13, 5));
                            if (!int.TryParse(ZipCode, out int zip))
                            {
                                errorMessage = "Patient zip code is required";
                            }
                            patientEntity.ZipCode = zip;


                        }
                        else if (line.StartsWith("PHONE") && line.Contains("CELL"))
                        {
                            var phone = line.Substring(5, 15);
                            if (string.IsNullOrEmpty(phone))
                            {
                                errorMessage = "Patient phone number required";
                            }
                            phone = phone.Trim();
                            phone = phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim();
                            if (!long.TryParse(phone, out long phoneNumber))
                            {
                                errorMessage = "Patient phone number required";
                            }
                            patientEntity.PhoneNumber = phoneNumber.ToString();
                        }

                        else if (line.StartsWith("GENDER"))
                        {
                            var gender = line.Substring(7, 1);
                            if (gender.Equals("M", StringComparison.InvariantCultureIgnoreCase))
                            {
                                patientEntity.Gender = GenderEnum.Male;
                            }
                            if (gender.Equals("F", StringComparison.InvariantCultureIgnoreCase))
                            {
                                patientEntity.Gender = GenderEnum.Female;
                            }

                        }
                        else if (line.StartsWith("MR#"))
                        {
                            var dob = line.Substring(28, 10);
                            var date = ConvertToDateTime(dob);
                            if (date != null)
                            {
                                patientEntity.DOB = date;
                            }


                        }
                        else if (line.StartsWith("PRIMARY DX") && line.Contains("(") && line.Contains(")"))
                        {
                            var diagnosis = line.Substring(line.IndexOf("(") + 1, line.IndexOf(")") - line.IndexOf("(") - 1);
                            patientEntity.DiagnosisCode1 = diagnosis;
                        }
                        else if (line.StartsWith("SECOND DX") && line.Contains("(") && line.Contains(")"))
                        {
                            var diagnosis = line.Substring(line.IndexOf("(") + 1, line.IndexOf(")") - line.IndexOf("(") - 1);
                            patientEntity.DiagnosisCode2 = diagnosis;
                        }

                        else if (line.StartsWith("EFFECTIVE DATE"))
                        {
                            var admissionDate = line.Substring("EFFECTIVE DATE".Length + 1, 10);
                            var date = ConvertToDateTime(admissionDate);
                            if (date != null)
                            {
                                patientEntity.AdmissionDate = date;
                            }
                        }
                        else if (line.StartsWith("MEDICAL DIRECTOR"))
                        {
                            var attendingPhyscian = line.Substring("MEDICAL DIRECTOR".Length, line.IndexOf(",") - "MEDICAL DIRECTOR".Length);
                            if (!string.IsNullOrEmpty(attendingPhyscian))
                            {
                                attendingPhyscian = attendingPhyscian.Trim();
                            }
                            patientEntity.AttendingPhysicianFullName = attendingPhyscian;
                            if (line.LastIndexOf("NPI") > 0)
                            {
                                var attendingPhysicanNPI = line.Substring(line.LastIndexOf("NPI:") + ("NPI:").Length, 10);
                                if (long.TryParse(attendingPhysicanNPI, out long attendingPhysicanNPIParsed))
                                {
                                    patientEntity.AttendingPhysicianNPI = attendingPhysicanNPIParsed;
                                }

                            }
                        }
                        else if (line.StartsWith("REFERRING/ATTENDING/PATIENT"))
                        {
                            var referringPhyscian = line.Substring("REFERRING/ATTENDING/PATIENT".Length, line.IndexOf(",") - "REFERRING/ATTENDING/PATIENT".Length);
                            if (!string.IsNullOrEmpty(referringPhyscian))
                            {
                                referringPhyscian = referringPhyscian.Trim();
                            }
                            patientEntity.ReferringPhysicianFullName = referringPhyscian;
                            if (line.LastIndexOf("NPI") > 0)
                            {
                                var referringPhysicanNPI = line.Substring(line.LastIndexOf("NPI:") + ("NPI:").Length, 10);
                                if (!long.TryParse(referringPhysicanNPI, out long referringPhysicanNPIParsed))
                                {
                                    // throw error
                                }
                                patientEntity.ReferringPhysicianNPI = referringPhysicanNPIParsed;
                            }
                        }
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            ShowPDFConfirmationDialog(true, $"Error while parsing with error as {errorMessage}");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + line);
            }

            if (string.IsNullOrEmpty(errorMessage))
            {
                ProviderEntitylFluentValidator providerValidator = new ProviderEntitylFluentValidator();
                var errorProviderResult = providerValidator.Validate(providerEntity);
                if (!errorProviderResult.IsValid && errorProviderResult.Errors.Count > 0)
                {
                    var error = "Provider " + errorProviderResult.Errors[0].ErrorMessage;

                    ShowPDFConfirmationDialog(true, error);
                    return;
                }
                else
                {
                    var providerDetails = await providerRepository.GetProviderByNameAndPhoneNumber(providerEntity.LegalName, providerEntity.PhoneNumber);
                    if (providerDetails == null || providerDetails.Count == 0)
                    {
                        await providerRepository.AddProvider(providerEntity);
                        patientEntity.Provider = providerEntity;
                    }
                    else
                    {
                        providerEntity.ProviderId = providerDetails[0].ProviderId;
                        patientEntity.Provider = providerDetails[0];
                    }
                }

                PatientEntitylFluentValidator patientValidator = new PatientEntitylFluentValidator();
                var errorPatientResult = patientValidator.Validate(patientEntity);
                if (!errorPatientResult.IsValid && errorPatientResult.Errors.Count > 0)
                {
                    var error = "Patient " + errorPatientResult.Errors[0].ErrorMessage;

                    ShowPDFConfirmationDialog(true, error);
                    return;
                }

                if (await patientRepository.AddPatient(patientEntity))
                {
                    var parameters = new DialogParameters<PDFConfirmation>();
                    parameters.Add(x => x.ContentText, $"Patient Details added : {patientEntity.FullName}");
                    parameters.Add(x => x.ButtonText, "Ok");
                    parameters.Add(x => x.Color, MudBlazor.Color.Success);

                    var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, DisableBackdropClick = true };

                    Dialog.Show<PDFConfirmation>("Result", parameters, options);
                };

            }

        }
        protected string GetStateName(string key)
        {
            if(stateDetails!.states.ContainsKey(key))
            return stateDetails!.states[key];

            return key;

		}

        private DateTime? ConvertToDateTime(string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                date = date.TrimStart().TrimEnd();
                if (date.Length == 9)
                {
                    date = "0" + date;
                }
            }
            try
            {
                var parsedDate = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.CurrentCulture);
                {
                   return parsedDate;
                }
            }
            catch (Exception ex)
            {
                // send error of parsing 
            }
            return null;

        }

        private void ShowPDFConfirmationDialog(bool isError, string message)
        {
            if (isError)
            {
                var parameters = new DialogParameters<PDFConfirmation>();
                parameters.Add(x => x.ContentText, $"{message}");
                parameters.Add(x => x.ButtonText, "Ok");
                parameters.Add(x => x.Color, MudBlazor.Color.Error);
                parameters.Add(x => x.IsError, true);
                
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, DisableBackdropClick = true };

                Dialog.Show<PDFConfirmation>("Result", parameters, options);
            }
            else
            {
                var parameters = new DialogParameters<PDFConfirmation>();
                parameters.Add(x => x.ContentText, $"{message}");
                parameters.Add(x => x.ButtonText, "Ok");
                parameters.Add(x => x.Color, MudBlazor.Color.Success);
                parameters.Add(x => x.IsError, false);
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, DisableBackdropClick = true };

                Dialog.Show<PDFConfirmation>("Result", parameters, options);
            }
        }
    }
}
