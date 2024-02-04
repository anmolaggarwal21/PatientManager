
using DeviceManager.Enums;
using DeviceManager.Migrations;
using DeviceManager.Pages.Provider;
using DeviceManager.Repository;
using DeviceManager.Shared;
using ElectronNET.API.Entities;
using Entities;
using IronXL;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MudBlazor;
using MudBlazor.Charts;
using OfficeOpenXml;
using SocketIOClient.Messages;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using LicenseContext = OfficeOpenXml.LicenseContext;

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

        protected void GetCobolFormat(PatientEntity patientEntity)
        {
            string mappingZipCode = "00000";
			string patientZipCode = "00000";
			string assemblyPath = Assembly.GetExecutingAssembly().Location;
			string assemblyDirectory = string.Empty;
			if (!string.IsNullOrEmpty(assemblyPath))
			{
				assemblyDirectory = System.IO.Path.GetDirectoryName(assemblyPath!);
				Console.WriteLine($"assembly Diectoy inisde if is {assemblyDirectory}");
			}
			else
			{
				assemblyDirectory = Directory.GetCurrentDirectory();
				Console.WriteLine($"assembly Diectoy inisde else is {assemblyDirectory}");
			}
			if (patientEntity.ZipCode != null && patientEntity.ZipCode != 0 && patientEntity.ZipCode.HasValue)
			{
				patientZipCode = patientEntity!.ZipCode!.Value.ToString();

			}
			try
			{
				string excelFile = System.IO.Path.Combine(assemblyDirectory!, "zipSearch.xlsx");
				FileInfo existingFile = new FileInfo(excelFile);
				ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(existingFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["search"];
                    worksheet.Cells.SetCellValue(2,2, patientEntity.ZipCode.ToString());
                    worksheet.Cells[2,2].Value = patientEntity.ZipCode;
                    
                    worksheet.Cells[4, 2].Calculate();
					mappingZipCode = worksheet.Cells[4,2].GetCellValue<string>();
                    
					
				}

				
				if (string.IsNullOrEmpty(mappingZipCode) || mappingZipCode.Equals("#N/A"))
				{
					mappingZipCode = "00000";
				}

				
			}
           catch(Exception ex)
            {
				Console.WriteLine($"Exception while reading excel with error as {ex.Message}");
			}
		
		
           
            
            string textFile = System.IO.Path.Combine(assemblyDirectory!, "templateFile.txt");
            Console.WriteLine($"TextFile is {textFile}");
            string attendingFirstName = string.Empty;
            string attendingLastName = string.Empty;
            string otherFirstName = string.Empty;
            string otherLastName = string.Empty;
            string patientLastName = string.Empty;
			string patientFirstName = string.Empty;

			if (!string.IsNullOrEmpty(patientEntity!.AttendingPhysicianFullName))
            {
                var commaSplitAttending = patientEntity!.AttendingPhysicianFullName.Split(",");

                if (commaSplitAttending.Length >= 2)
                {
					attendingFirstName = commaSplitAttending[1];
					attendingLastName = commaSplitAttending[0];
				}
                else
                {
                    var splitName = patientEntity!.AttendingPhysicianFullName.Split(" ");
					attendingLastName = splitName[0];
					if (splitName.Length >= 2)
                    {
                        
						attendingFirstName = splitName[1];
					}
                }
                    


            }
            if (!string.IsNullOrEmpty(patientEntity!.OtherPhysicianFullName))
            {
				var commaSplitOtherPhysician = patientEntity!.OtherPhysicianFullName.Split(",");

                if (commaSplitOtherPhysician.Length >= 2)
                {
					otherFirstName = commaSplitOtherPhysician[1];
					otherLastName = commaSplitOtherPhysician[0];
                }
                else
                {
                    var splitName = patientEntity!.OtherPhysicianFullName.Split(" ");
					otherLastName = splitName[0];
					if (splitName.Length >= 2)
                    {
                        
						otherFirstName = splitName[1];

					}
                }
            }

            if (!string.IsNullOrEmpty(patientEntity.FullName))
            {
                var fullNameSplit = patientEntity.FullName.Split(',');

				if (fullNameSplit.Length > 1)
                {
					patientLastName = fullNameSplit[0];
                    patientFirstName = fullNameSplit[1];

                }
                else
                {
					patientFirstName = fullNameSplit[0];
				}
            }
            Dictionary<string, string?> keyValuePairs = new Dictionary<string, string?>();
            keyValuePairs.Add("[$PALM$]", patientLastName);
            keyValuePairs.Add("[$PAFM$]", patientFirstName);
            keyValuePairs.Add("[$PDOB$]", patientEntity!.DOB!.GetValueOrDefault().Date.ToString("MMddyyyy"));
            keyValuePairs.Add("[$DR$]", patientEntity!.Address ?? "");
            keyValuePairs.Add("[$TY$]", patientEntity!.City ?? "");
            keyValuePairs.Add("[$PATIENTSTATE$]", patientEntity!.state ?? "");
            keyValuePairs.Add("[$PDIG1$]", string.IsNullOrEmpty(patientEntity!.DiagnosisCode1) ? "" : patientEntity!.DiagnosisCode1.Replace(".",""));
            keyValuePairs.Add("[$PDIG2$]", string.IsNullOrEmpty(patientEntity!.DiagnosisCode2) ? "" : patientEntity!.DiagnosisCode2.Replace(".", ""));
            keyValuePairs.Add("[$PDIG3$]", string.IsNullOrEmpty(patientEntity!.DiagnosisCode3) ? "" : patientEntity!.DiagnosisCode3.Replace(".", ""));
            keyValuePairs.Add("[$PDIG4$]", string.IsNullOrEmpty(patientEntity!.DiagnosisCode4) ? "" : patientEntity!.DiagnosisCode4.Replace(".", ""));
            keyValuePairs.Add("[$PDIG5$]", string.IsNullOrEmpty(patientEntity!.DiagnosisCode5) ? "" : patientEntity!.DiagnosisCode5.Replace(".", ""));
            keyValuePairs.Add("[$PDIG6$]", string.IsNullOrEmpty(patientEntity!.DiagnosisCode6) ? "" : patientEntity!.DiagnosisCode6.Replace(".", ""));
            keyValuePairs.Add("[$PANPI$]", patientEntity!.AttendingPhysicianNPI != 0 ? patientEntity!.AttendingPhysicianNPI.ToString(): "");
            keyValuePairs.Add("[$PALNAME$]", attendingLastName);
            keyValuePairs.Add("[$PAFNA$]", attendingFirstName );
            keyValuePairs.Add("[$PONPI$]", patientEntity!.OtherPhysicianNPI != 0 ? patientEntity!.OtherPhysicianNPI.ToString() : "");
            keyValuePairs.Add("[$POLNAME$]", otherLastName);
            keyValuePairs.Add("[$POFNA$]", otherFirstName);
            keyValuePairs.Add("[$PRONPI]", patientEntity.Provider.NPI == null ? "" : patientEntity.Provider.NPI.ToString());
            keyValuePairs.Add("[$PAD]", patientEntity.AdmissionDate.GetValueOrDefault().Date.ToString("MMddyy"));
            keyValuePairs.Add("[$PZ]", patientZipCode);
            keyValuePairs.Add("[$PAZIPMAP]", $"{patientZipCode}-{mappingZipCode}");
            keyValuePairs.Add("[$S]", $"EX {patientEntity.Gender.ToString().Substring(0,1)}");
            Dictionary<string, string> replacedNewValue = new Dictionary<string, string>();
            StringBuilder newLine= new StringBuilder();
            
            if (File.Exists(textFile))
            {
                // Read a text file line by line.
                string[] lines = File.ReadAllLines(textFile);
                int length = 0;
                bool startReplacing = false;
                StringBuilder replacedWord = new StringBuilder();
                StringBuilder newValue = new StringBuilder();
               
                foreach (string line in lines)
                {
                    int lineLength = line.Length;
                    var a = line;
                    foreach (char c in line)
                    {
                        lineLength--;

                        if (c == '[' )
                        {
                            replacedWord = new StringBuilder();
                            newValue = new StringBuilder();
                            length = 1;
                            replacedWord.Append(c);
                        }
                        else if (c == ']')
                        {
                            replacedWord.Append(c);
                            startReplacing = true;
                        }

                        else if ((c != ' ' || lineLength ==0 ) && length > 0 && startReplacing )
                        {
                            // replace here
                            if(keyValuePairs.ContainsKey(replacedWord.ToString().Trim()))
                            {
                                var value = keyValuePairs[replacedWord.ToString().Trim()];

                               
                                if (value != null  && replacedWord.ToString().Length > value.Length)
                                {

                                     newValue.Append(value);
                                    
                                   
                                    for (int i = 0; i < replacedWord.ToString().Length - value.Length; i++)
                                    {
                                        newValue!.Append(" ");
                                    }
                                }
                              
                                if(value != null && replacedWord.ToString().Length <= value.Length)
                                {
                                    newValue.Append(value.Substring(0, replacedWord.ToString().Length - 1));
                                    newValue.Append(" ");
                                }
                                if(newValue != null && !string.IsNullOrEmpty(newValue.ToString()))
                                {
                                   a =  a.Replace(replacedWord.ToString(), newValue.ToString());
                                  
                                
                                }
                               
                            }
                            startReplacing = false;
                            length = 0;
                        }
                        else if(length >0)
                        {
                            replacedWord.Append(c);
                        }
                         if(lineLength ==0)
                        {
                            replacedWord = new StringBuilder();
                            newValue = new StringBuilder();
                            length = 0;
                            startReplacing = false;
                            newLine.AppendLine(a);
                        }
                        if(length >= 1)
                        {
                            length++;
                        }
                    }
                    
                }
                ShowPDFConfirmationDialog(false, newLine.ToString(), true);
            }
            
        }
        protected async void UploadFiles2(IBrowserFile file)
        {
             string fileContent = "";
             files.Add(file);

            long maxsize = 512000;

            var buffer = new byte[file.Size];
            //await file.OpenReadStream(maxsize);
            var copy = new MemoryStream();
            Stream stream = file.OpenReadStream();
            await stream.CopyToAsync(copy);
            copy.Position = 0;
           PdfReader reader = new PdfReader(copy);
                string text = string.Empty;
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    fileContent += PdfTextExtractor.GetTextFromPage(reader, page);
                }
                reader.Close();
            
                //fileContent = System.Text.Encoding.UTF8.GetString(buffer);

                
            await ParsePatientAndProviderText(fileContent);
            //TODO upload the files to the server
        }

        private async Task ParsePatientAndProviderText(string result)
        {
            string line= string.Empty;
            string[] lines = result.Split("\n");
            PatientEntity patientEntity = new PatientEntity();
            ProviderEntity providerEntity = new ProviderEntity();
            string errorMessage = string.Empty;
            try
            {
                if (lines != null && lines.Length >= 2)
                {
                    var providerName = lines[0];

                    #region Rrovider Details
                    if (string.IsNullOrEmpty(providerName))
                    {
                        errorMessage = "Provider Name is required";
                        ShowPDFConfirmationDialog(true, $"Error while parsing with error as {errorMessage}");
                        return;
                    }
                    providerEntity.LegalName = providerName;
                    string providerDetails = string.Empty;

                    if (string.IsNullOrEmpty(lines[1]) || string.IsNullOrWhiteSpace(lines[1]))
                    {
                        providerDetails = lines[2];
                    }
                    else
                    {
                        providerDetails = lines[1];
                    }
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
                        if(telephoneSplit != null && telephoneSplit.Length >= 1)
                        {
                            var number = telephoneSplit[1].Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                            var phone = number.Substring(0, 10);
                            if (long.TryParse(phone, out long phoneNumber))
                            {
                                providerEntity.PhoneNumber = phoneNumber.ToString();
                            }
                            else
                            {
                                providerEntity.PhoneNumber = "9999999999";
                            }
                        }
                        else
                        {
                            providerEntity.PhoneNumber = "9999999999";
                        }
                        
                        providerEntity.State = state.Trim();
                        providerEntity.City = city.Trim();
                        
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
                                patientEntity.PhoneNumber = "9999999999";
                            }
                            else
                            {


                                phone = phone.Trim();
                                phone = phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Trim();
                                if (!long.TryParse(phone, out long phoneNumber))
                                {
                                    patientEntity.PhoneNumber = "9999999999";
                                }
                                else
                                {
                                    patientEntity.PhoneNumber = phoneNumber.ToString();
                                }
                                
                            }
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
                           if (line.IndexOf("DOB") > 0)
                            {
                                var dob = line.Substring(line.IndexOf("DOB") + 3, 11);
                                var date = ConvertToDateTime(dob);
                                if (date != null)
                                {
                                    patientEntity.DOB = date;
                                }
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
                            else
                            {
                                patientEntity.AdmissionDate = DateTime.Now;
                            }
                        }
                        else if (line.StartsWith("MEDICAL DIRECTOR"))
                        {
                            var otherPhyscian = line.Substring("MEDICAL DIRECTOR".Length, line.IndexOf("Phone") - "MEDICAL DIRECTOR".Length);
                           
                            if (!string.IsNullOrEmpty(otherPhyscian))
                            {
								otherPhyscian = otherPhyscian.Trim();
								if (otherPhyscian.EndsWith(","))
                                {
                                    otherPhyscian = otherPhyscian.Substring(0, otherPhyscian.Length - 1);
                                }
								otherPhyscian = otherPhyscian.Trim();
                            }
                            patientEntity.OtherPhysicianFullName = otherPhyscian;
                            if (line.LastIndexOf("NPI") > 0)
                            {
                                var otherPhysicanNPI = line.Substring(line.LastIndexOf("NPI:") + ("NPI:").Length, 10);
                                if (long.TryParse(otherPhysicanNPI, out long otherPhysicanNPIParsed))
                                {
                                    patientEntity.OtherPhysicianNPI = otherPhysicanNPIParsed;
                                }

                            }
                        }
                        else if (line.StartsWith("REFERRING/ATTENDING/PATIENT"))
                        {
                            var attendingPhyscian = line.Substring("REFERRING/ATTENDING/PATIENT".Length, line.IndexOf("Phone") - "REFERRING/ATTENDING/PATIENT".Length);
                            if (!string.IsNullOrEmpty(attendingPhyscian))
                            {
								attendingPhyscian = attendingPhyscian.Trim();
								if (attendingPhyscian.EndsWith(","))
								{
									attendingPhyscian = attendingPhyscian.Substring(0, attendingPhyscian.Length - 1);
								}
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
                if(patientEntity.AdmissionDate == null)
                {
                    patientEntity.AdmissionDate = DateTime.Now;
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

        private void ShowPDFConfirmationDialog(bool isError, string message, bool showCopy = false)
        {
            if (isError)
            {
                var parameters = new DialogParameters<PDFConfirmation>();
                parameters.Add(x => x.ContentText, $"{message}");
                parameters.Add(x => x.ButtonText, "Ok");
                parameters.Add(x => x.Color, MudBlazor.Color.Error);
                parameters.Add(x => x.IsError, true);
                parameters.Add(x => x.ShowCopy, showCopy);
                
                var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, DisableBackdropClick = true };

                Dialog.Show<PDFConfirmation>("Result", parameters, options);
            }
            else
            {
                var parameters = new DialogParameters<PDFConfirmation>();
                parameters.Add(x => x.ContentText, $"{message}");
                parameters.Add(x => x.ButtonText, "Ok");
                parameters.Add(x => x.Color, MudBlazor.Color.Success);
                parameters.Add(x => x.IsError, false);
                parameters.Add(x => x.ShowCopy, showCopy);
                var options = new DialogOptions();
                if (showCopy)
                {
                     options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, DisableBackdropClick = true };
					Dialog.Show<PDFConfirmation>("", parameters, options);
				}
                else
                {
                     options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Large, DisableBackdropClick = true };
					Dialog.Show<PDFConfirmation>("Result", parameters, options);
				}
                

              
            }
        }

        protected async void OpenProviderDetails(Guid providerId)
        {
            var parameters = new DialogParameters<ProviderDetailsDialog>();
            parameters.Add(x => x.Id, providerId.ToString());

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium, DisableBackdropClick = true, FullWidth = true };

            Dialog.Show<ProviderDetailsDialog>("Provider Details", parameters, options);
        }

        protected async void OpenPatientDetails(PatientEntity patientEntity)
        {
            var parameters = new DialogParameters<PatientDetailsDialog>();
            parameters.Add(x => x.patientEntity, patientEntity);

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.Medium, DisableBackdropClick = true, FullWidth = true };

            Dialog.Show<PatientDetailsDialog>("Patient Details", parameters, options);
        }

        
    }
}
