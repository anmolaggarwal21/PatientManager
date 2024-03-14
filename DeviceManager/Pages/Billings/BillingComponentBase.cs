using DeviceManager.Enums;
using DeviceManager.Repository;
using DeviceManager.Shared;
using Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Nextended.Core.Types;
using OfficeOpenXml;
using System.Globalization;
using Color = MudBlazor.Color;

namespace DeviceManager.Pages.Billing
{
    public class BillingComponentBase : Microsoft.AspNetCore.Components.ComponentBase
    {
        public List<BillingEntityDto>? billingEntityDto { get; set; }
        protected string searchString1 = "";
        protected BillingEntityDto selectedItem1 = null;
        public HashSet<BillingEntityDto> selectedItems = new HashSet<BillingEntityDto>();
        protected MudTable<BillingEntityDto> mudTable;
        DialogOptions disableBackdropClick = new DialogOptions() { DisableBackdropClick = true };
        protected bool pageLoaded { get; set; } = false;
        [Inject]
        protected IDialogService Dialog { get; set; } 
        [Inject]
        protected IBillingRepository billingRepository { get; set; }
        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject] public StateDetails? stateDetails { get; set; }
        public bool FilterFunc1(BillingEntityDto element) => FilterFunc(element, searchString1);

        private bool FilterFunc(BillingEntityDto element, string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
                        
            if ( !string.IsNullOrEmpty(element.PatientName) && element.PatientName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        protected override async Task OnInitializedAsync() 
        {
            billingEntityDto = new List<BillingEntityDto>();
            var list = await billingRepository.GetAllBilling();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    billingEntityDto.Add((BillingEntityDto)item);
                }
            }
            await base.OnInitializedAsync();
            pageLoaded = true;
        }

        protected async void UploadFiles2(IBrowserFile file)
        {
            long maxsize = 512000;

            var buffer = new byte[file.Size];
            //await file.OpenReadStream(maxsize);
            var copy = new MemoryStream();
            Stream stream = file.OpenReadStream();
            await stream.CopyToAsync(copy);
            copy.Position = 0;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Dictionary<string, int> headerValuePairs = new Dictionary<string, int>();
            using (ExcelPackage package = new ExcelPackage(copy))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Billing"];
                if (worksheet != null)
                {
                    var rowCount = worksheet.Rows.Count();
                    var columns = worksheet.Columns.ToList();
                    for (int i = 1; i <= columns.Count; i++)
                    {
                        var columnHeader = worksheet.Cells[1, i].Value;
                        if (columnHeader != null)
                        {
                            headerValuePairs.Add(columnHeader.ToString()!, i);
                        }
                    }

                    for (int i = 2; i <= rowCount; i++)
                    {
                        BillingEntity entity = new BillingEntity();
                        entity.BillingItem = new BillingItem();
                        entity.BillingItem.POSBillingItem = new BillingData();
                        entity.BillingItem.POSBillingItem.Data = new List<Entities.Data>();
                        entity.BillingItem.OtherBillingItems = new List<BillingData>();
                        entity.FoundPatient = true;
                        for (int j = 0; j < BillingConstantClass.BillingType.Count; j++)
                        {
                            var type = BillingConstantClass.BillingType[j];
                            if (headerValuePairs.TryGetValue(type, out int index))
                            {
                                var valueStoredInExcel = worksheet.Cells[i, index].Value;
                                if (valueStoredInExcel != null && !string.IsNullOrEmpty(valueStoredInExcel.ToString()))
                                {
                                    var valueStoredinExcelString = valueStoredInExcel.ToString()!.Trim();
                                    switch (type.ToUpper())
                                    {
                                        case "PATIENT":
                                            var patientValueAfterSplit = valueStoredinExcelString!.Split(",");
                                            if (patientValueAfterSplit.Length >= 2)
                                            {
                                                int admisionIndex = 3;
                                                if (patientValueAfterSplit[1].Contains("MR#"))
                                                {
                                                    admisionIndex = 2;
                                                    var patientName = patientValueAfterSplit[0];
                                                    entity.PatientName = patientName;
                                                }
                                                else
                                                {
                                                    var patientName = patientValueAfterSplit[0] + "," + patientValueAfterSplit[1];
                                                    entity.PatientName = patientName;
                                                }
                                                var admissionDate = patientValueAfterSplit[admisionIndex].Split("-")[0].Trim();
                                                entity.AdmissionDate = DateTime.ParseExact(admissionDate, "MM/dd/yyyy", CultureInfo.CurrentCulture);
                                            }

                                            break;

                                        case "POS":
                                            entity.BillingItem.POSBillingItem.Type = valueStoredinExcelString!;

                                            break;
                                        case "CARE DAY":
                                            entity.BillingItem.POSBillingItem.Data.Add(new Entities.Data(int.Parse(valueStoredinExcelString!), entity.AdmissionDate.GetValueOrDefault()));
                                            break;
                                        case "PAYER":
                                            entity.Payer = valueStoredinExcelString;
                                            break;
                                        case "R&B":
                                            break;
                                        case "MD VISITS":
                                        case "RN VISITS":
                                        case "LVN VISITS":
                                        case "HA VISITS":
                                        case "SW VISITS":
                                        case "SC VISITS":
                                            var colonSplit = valueStoredinExcelString;
                                            var indexOfColon = valueStoredinExcelString.IndexOf(":");
                                            if (indexOfColon > 0)
                                            {
                                                indexOfColon++;
                                                colonSplit = valueStoredinExcelString.Substring(indexOfColon, valueStoredinExcelString.Length - indexOfColon);
                                            }
                                            var colonCommaSplit = colonSplit.Split(",");
                                            for (int l = 0; l < colonCommaSplit.Length; l++)
                                            {
                                                var typeValue = colonCommaSplit[l];
                                                if (typeValue.EndsWith("."))
                                                {
                                                    typeValue = typeValue.Substring(0, typeValue.Length - 1);
                                                }
                                                var monthSplit = typeValue.Split('/');
                                                var month = monthSplit[0].Trim().Length == 1 ? $"0{monthSplit[0].Trim()}" : monthSplit[0].Trim();
                                                var date = monthSplit[1].Split("-")[0].Trim().Length == 1 ? $"0{monthSplit[1].Split("-")[0].Trim()}" : monthSplit[1].Split("-")[0].Trim();
                                                var duration = monthSplit[1].Split("-")[1].Trim();
                                                var dataToBeAdded = new Entities.Data(int.Parse(duration), DateTime.ParseExact($"{month}/{date}/{DateTime.Now.Year}".Trim(), "MM/dd/yyyy", CultureInfo.CurrentCulture));
                                                var existingType = entity.BillingItem.OtherBillingItems.Find(x => x.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
                                                if (existingType != null)
                                                {
                                                    existingType.Data.Add(dataToBeAdded);

                                                }
                                                else
                                                {
                                                    entity.BillingItem.OtherBillingItems.Add(new BillingData() { Type = type, Data = new List<Entities.Data>() { dataToBeAdded } });
                                                }
                                            }


                                            break;
                                        case "SW PHONE":
                                            break;
                                    }
                                }
                            }
                        }

                        await billingRepository.AddBilling(entity);
                    }
                }


            }
        }

        protected void OnShowDetails(Guid id)
        {
            var billingDetails = this.billingEntityDto!.Find(x => x.BillingId.Equals(id));
            if (billingDetails != null)
            {
                if (billingDetails.ShowDetails)
                {
                    billingDetails.ShowDetails = false;
                    return;
                }
                else
                {
                    billingDetails.ShowDetails = true;
                }
                if (billingDetails.BillingItem != null && billingDetails.TypeDetails!.Count == 0)
                {

                    if (billingDetails.BillingItem.POSBillingItem != null)
                    {
                        var type = billingDetails.BillingItem.POSBillingItem.Type;
                        if (billingDetails.BillingItem.POSBillingItem.Data != null && billingDetails.BillingItem.POSBillingItem.Data.Count > 0)
                        {
                            foreach (var item in billingDetails.BillingItem.POSBillingItem.Data)
                            {
                                decimal totalAmount = 0;
                                string billingCode = "";
                                var billingCodeDetails = BillingConstantClass.BillingCodeAndAmount.Find(x => x.BillingType.Equals(type, StringComparison.OrdinalIgnoreCase));
                                if (billingCodeDetails != null)
                                {
                                    totalAmount = billingCodeDetails.IsCostPerDay ? (item.NoOfDays * billingCodeDetails.BillingAmount) : billingCodeDetails.BillingAmount;
                                    billingCode = $"{billingCodeDetails.RevenueBillingCode} {billingCodeDetails.HCPCBillingCode}";
                                }
                                var startDate = item.NoOfDays;
                                var endDate = item.NoOfDays;
                                var admissionDate = item.AdmissionDate;
                                billingDetails.TypeDetails!.Add(new TypeDetails()
                                {
                                    AdmissionDate = admissionDate!.Value.Date.ToString("MM-dd-yyyy"),
                                    Amount = totalAmount,
                                    Code = billingCode,
                                    NoOfDays1 = startDate.ToString(),
                                    NoOfDays2 = startDate.ToString(),
                                    Type = type

                                });
                            }
                        }

                    }
                    if (billingDetails.BillingItem.OtherBillingItems != null && billingDetails.BillingItem.OtherBillingItems.Count > 0)
                    {
                        foreach (var billingItem in billingDetails.BillingItem.OtherBillingItems)
                        {
                            var otherBillingType = billingItem.Type;

                            foreach (var item in billingItem.Data)
                            {
                                var startDate = item.NoOfDays;
                                var endDate = item.NoOfDays;
                                var admissionDate = item.AdmissionDate;
                                decimal totalAmount = 0;
                                string billingCode = "";
                                var billingCodeDetails = BillingConstantClass.BillingCodeAndAmount.Find(x => x.BillingType.Equals(otherBillingType, StringComparison.OrdinalIgnoreCase));
                                if (billingCodeDetails != null)
                                {
                                    totalAmount = billingCodeDetails.IsCostPerDay ? (item.NoOfDays * billingCodeDetails.BillingAmount) : billingCodeDetails.BillingAmount;
                                    billingCode = $"{billingCodeDetails.RevenueBillingCode} {billingCodeDetails.HCPCBillingCode}";
                                }
                                billingDetails.TypeDetails!.Add(new TypeDetails()
                                {
                                    AdmissionDate = admissionDate!.Value.Date.ToString("MM-dd-yyyy"),
                                    Amount = totalAmount,
                                    Code = billingCode,
                                    NoOfDays1 = startDate.ToString(),
                                    NoOfDays2 = startDate.ToString(),
                                    Type = otherBillingType

                                });
                            }
                        }
                    }
                }
            }

            foreach (var item in billingEntityDto)
            {
                if (billingDetails != null && item.BillingId != billingDetails.BillingId)
                {
                    item.ShowDetails = false;
                }
            }
        }

        protected void OnEditClick(Guid id)
        {
            var b = mudTable.SelectedItem;
            var a = selectedItem1;
            navigationManager.NavigateTo($"/EditBilling/{id}");
        }


        protected void OnDeleteClick(Guid id)
        {
            var parameters = new DialogParameters<DeleteDialog>();
            parameters.Add(x => x.ContentText, "Do you really want to delete these billing? This process cannot be undone.It is a cascading delete");
            parameters.Add(x => x.ButtonText, "Delete");
            parameters.Add(x => x.Color, Color.Error);
            parameters.Add(x => x.entityTypeEnum, EntityTypeEnum.Provider);
            parameters.Add(x => x.Id, id.ToString());

            var options = new DialogOptions() { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, DisableBackdropClick = true };

            Dialog.Show<DeleteDialog>("Delete", parameters, options);
        }
        protected void OnPatientClick(Guid id)
        {
            var legalName = billingEntityDto?.Find(x => x.BillingId == id)?.PatientName;
            navigationManager.NavigateTo($"/Patient/{id}");
        }

        protected string GetStateName(string key)
        {
            if (stateDetails!.states.ContainsKey(key))
                return stateDetails!.states[key];

            return key;

        }
    }
}
