using DeviceManager.ConstantClass;
using DeviceManager.Enums;
using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Nextended.Core.Types;
using System.Globalization;
using Color = MudBlazor.Color;

namespace DeviceManager.Shared
{
    public class BillingConsoleOutputBase : ComponentBase
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter] public Guid BillingId { get; set; }

        [Parameter] public string ButtonText { get; set; }

        [Parameter] public Color Color { get; set; }
        public List<BillingAmount> billingAmountDetails { get; set; } = new List<BillingAmount>();

        [Inject]
        NavigationManager navigationManager { get; set; }
        [Inject]
        protected IBillingRepository billingRepository { get; set; }    


        protected override async Task OnInitializedAsync()
        {
           var billingEntity= await billingRepository.GetBillingById(BillingId);

            AddToBillingAmountDetails(billingEntity.PatientDetails, billingEntity.POS, billingEntity.CareDay);
            AddToBillingAmountDetailsForDifferentType(billingEntity.RAndB, BillingConstantClass.rnvConstant);
            AddToBillingAmountDetailsForDifferentType(billingEntity.MDVisits, BillingConstantClass.rnvConstant);
            AddToBillingAmountDetailsForDifferentType(billingEntity.RNVisits, BillingConstantClass.rnvConstant);
            AddToBillingAmountDetailsForDifferentType(billingEntity.LVNVisits, BillingConstantClass.lvnConstant);
            AddToBillingAmountDetailsForDifferentType(billingEntity.HAVisits, BillingConstantClass.haConstant);
            AddToBillingAmountDetailsForDifferentType(billingEntity.SWVisits, BillingConstantClass.mswConstant);
            AddToBillingAmountDetailsForDifferentType(billingEntity.SWWPHone, BillingConstantClass.mswConstant);
            AddToBillingAmountDetailsForDifferentType(billingEntity.SCVisists, BillingConstantClass.mswConstant);
            AddToBillingAmountDetailsForDifferentType("RX", BillingConstantClass.rxConstant);
            AddToBillingAmountDetailsForDifferentType("TotalCharge", BillingConstantClass.TotalChargeConstant);
            await base.OnInitializedAsync();
        }

        private void AddToBillingAmountDetails(string patientDetail, string pos, int careDay)
        {
           var codeAndAmount = BillingConstantClass.BillingCodeAndAmount.Find(x => x.BillingType.Equals(pos, StringComparison.OrdinalIgnoreCase));
            if(codeAndAmount != null)

            {
                long amount = codeAndAmount.BillingAmount;
                if (codeAndAmount.IsCostPerDay)
                {
                    amount = codeAndAmount.BillingAmount * careDay;
                }
                billingAmountDetails.Add(new BillingAmount
                {
                    StartNoOfDays = careDay,
                    EndNoOfDays = careDay,
                    AdmitDate = DateTime.Now,
                    Amount = amount,
                    Code = $"{codeAndAmount.HCPCBillingCode} {codeAndAmount.RevenueBillingCode}"
                });
            }

         
        }
        private void AddToBillingAmountDetailsForDifferentType(string typeValue, string BillingType)
        {
            if (string.IsNullOrEmpty(typeValue))
                return;
            var codeAndAmount = BillingConstantClass.BillingCodeAndAmount.Find(x => x.BillingType.Equals(BillingType, StringComparison.OrdinalIgnoreCase));
            if (codeAndAmount == null)
            {
                return;
            }
            switch (BillingType)
            {
                case BillingConstantClass.rnvConstant:
                case BillingConstantClass.lvnConstant:
                case BillingConstantClass.mswConstant:
                case BillingConstantClass.swConstant:
                case BillingConstantClass.mswPhoneCall:
                case BillingConstantClass.haConstant:

                    var colonSplit = typeValue;
                    var indexOfColon = typeValue.IndexOf(":");
                    if (indexOfColon > 0)
                    {
                        indexOfColon++;
                        colonSplit = typeValue.Substring(indexOfColon, typeValue.Length - indexOfColon);
                    }
                    var colonCommaSplit = colonSplit.Split(",");
                    for (int l = 0; l < colonCommaSplit.Length; l++)
                    {
                        var typeValueDetails = colonCommaSplit[l];
                        if (typeValueDetails.EndsWith("."))
                        {
                            typeValueDetails = typeValueDetails.Substring(0, typeValueDetails.Length - 1);
                        }
                        var monthSplit = typeValueDetails.Split('/');
                        var month = monthSplit[0].Trim().Length == 1 ? $"0{monthSplit[0].Trim()}" : monthSplit[0].Trim();
                        var date = monthSplit[1].Split("-")[0].Trim().Length == 1 ? $"0{monthSplit[1].Split("-")[0].Trim()}" : monthSplit[1].Split("-")[0].Trim();
                        var duration = monthSplit[1].Split("-")[1].Trim();


                        billingAmountDetails.Add(new BillingAmount
                        {
                            StartNoOfDays = int.Parse(duration),
                            EndNoOfDays = int.Parse(duration),
                            AdmitDate = DateTime.ParseExact($"{month}/{date}/{DateTime.Now.Year}".Trim(), "MM/dd/yyyy", CultureInfo.CurrentCulture),
                            Amount = codeAndAmount.BillingAmount,
                            Code = $"{codeAndAmount.HCPCBillingCode} {codeAndAmount.RevenueBillingCode}"
                        });

                    }

                    break;
                case BillingConstantClass.rxConstant:
                    billingAmountDetails.Add(new BillingAmount
                    {
                        StartNoOfDays = int.Parse("1"),
                        EndNoOfDays = int.Parse("1"),
                        Amount = codeAndAmount.BillingAmount,
                        Code = $"{codeAndAmount.HCPCBillingCode} {codeAndAmount.RevenueBillingCode}"
                    });
                    break;
                case BillingConstantClass.TotalChargeConstant:
                    var sumAmount = billingAmountDetails.Sum(x => x.Amount);  
                    billingAmountDetails.Add(new BillingAmount
                    {
                        
                        Amount = sumAmount,
                        Code = $"{codeAndAmount.HCPCBillingCode} {codeAndAmount.RevenueBillingCode}"
                    });
                    break;
            }
        }

                    
        protected async Task Submit()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }

       
        protected void Cancel() => MudDialog.Cancel();
        protected double startX, startY, offsetX, offsetY;

        protected void OnDragStart(DragEventArgs args)
        {
            startX = args.ClientX;
            startY = args.ClientY;
        }

        protected void OnDragEnd(DragEventArgs args)
        {
            offsetX += args.ClientX - startX;
            offsetY += args.ClientY - startY;
        }
    }
}
