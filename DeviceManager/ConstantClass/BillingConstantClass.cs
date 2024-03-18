using static MudBlazor.Colors;

namespace DeviceManager.ConstantClass
{
    public class BillingConstantClass
    {
        public const string rnvConstant = "RN VISITS";
        public const string lvnConstant = "LVN VISITS";
        public const string mswConstant = "MSW VISITS";
        public const string swConstant = "SW VISITS";
        public const string mswPhoneCall = "MSW phone call VISITS";
        public const string haConstant = "HHA VISITS";
        public const string rxConstant = "RX";
        public const string TotalChargeConstant = "Total Charges VISITS";





        public static List<string> BillingType = new List<string>() {

         "PATIENT",
         "POS",
         "CARE DAY",
         "R&B",
         "MD VISITS",
         "RN VISITS",
         "LVN VISITS",
         "HA VISITS",
         "SW VISITS",
         "SW PHONE",
         "SC VISITS",
         "NOTES / AUTH_NO"
     };

        public static List<BillingCodeAndAmount> BillingCodeAndAmount = new List<BillingCodeAndAmount>()
     {
         new BillingCodeAndAmount()
         {
             BillingType= "Home",
             RevenueBillingCode = "0651",
             BillingAmount= 200,
             IsCostPerDay = true,
             HCPCBillingCode = "Q5001"

         },
         new BillingCodeAndAmount()
         {
             BillingType= "Assisted living facility",
             RevenueBillingCode = "0652",
             BillingAmount= 200,
             IsCostPerDay = true,
             HCPCBillingCode = "Q5002"

         },
         new BillingCodeAndAmount()
         {
             BillingType= "Nursing long term care facility (LTC) or non-skilled nursing facility (NF)",
             RevenueBillingCode = "0655",
             BillingAmount= 200,
             IsCostPerDay = true,
             HCPCBillingCode = "Q5003"

         },
         new BillingCodeAndAmount()
         {
             BillingType= "Skilled nursing facility (SNF)",
             RevenueBillingCode = "0656",
             BillingAmount= 200,
             IsCostPerDay = true,
             HCPCBillingCode = "Q5004"

         },
         new BillingCodeAndAmount()
         {
             BillingType= "Inpatient hospital",
             RevenueBillingCode = "0658",
             BillingAmount= 200,
             IsCostPerDay = true,
             HCPCBillingCode = "Q5005"

         },
         new BillingCodeAndAmount()
         {
             BillingType= "Hospice facility ( house )",
             RevenueBillingCode = "",
             BillingAmount= 200,
             IsCostPerDay = true,
             HCPCBillingCode = "Q5006"

         },
         new BillingCodeAndAmount()
         {
             BillingType= "Long term care facility",
             RevenueBillingCode = "",
             BillingAmount= 200,
             IsCostPerDay = true,
             HCPCBillingCode = "Q5007"

         },
         new BillingCodeAndAmount()
         {
             BillingType= "Inpatient psychiatric facility",
             RevenueBillingCode = "",
             BillingAmount= 200,
             IsCostPerDay = true,
             HCPCBillingCode = "Q5008"

         },
         new BillingCodeAndAmount()
         {
             BillingType= "Place not otherwise specified (NOS)",
             RevenueBillingCode = "",
             BillingAmount= 200,
             IsCostPerDay = true,
             HCPCBillingCode = "Q5009"

         },
         new BillingCodeAndAmount()
         {
             BillingType= rnvConstant,
             RevenueBillingCode = "0551",
             BillingAmount= 10,
             IsCostPerDay = false,
             HCPCBillingCode = "G0299"

         },
         new BillingCodeAndAmount()
         {
             BillingType= lvnConstant,
             RevenueBillingCode = "0551",
             BillingAmount= 10,
             IsCostPerDay = false,
             HCPCBillingCode = "G0300"

         },
         new BillingCodeAndAmount()
         {
             BillingType= mswConstant,
             RevenueBillingCode = "0561",
             BillingAmount= 10,
             IsCostPerDay = false,
             HCPCBillingCode = "G0155"

         },
         new BillingCodeAndAmount()
         {
             BillingType= swConstant,
             RevenueBillingCode = "0561",
             BillingAmount= 10,
             IsCostPerDay = false,
             HCPCBillingCode = "G0155"

         },
         new BillingCodeAndAmount()
         {
             BillingType= mswPhoneCall,
             RevenueBillingCode = "0569",
             BillingAmount= 10,
             IsCostPerDay = false,
             HCPCBillingCode = "G0155"
         },
         new BillingCodeAndAmount()
         {
             BillingType= "Physical Therapy VISITS",
             RevenueBillingCode = "0421",
             BillingAmount= 10,
             IsCostPerDay = false,
             HCPCBillingCode = "G0151"
         },
         new BillingCodeAndAmount()
         {
             BillingType= "Occupational Therapy VISITS",
             RevenueBillingCode = "0431",
             BillingAmount= 10,
             IsCostPerDay = false,
             HCPCBillingCode = "G0152"
         },
         new BillingCodeAndAmount()
         {
             BillingType= "Speech Therapy VISITS",
             RevenueBillingCode = "0441",
             BillingAmount= 10,
             IsCostPerDay = false,
             HCPCBillingCode = "G0153"
         },
         new BillingCodeAndAmount()
         {
             BillingType= haConstant,
             RevenueBillingCode = "0571",
             BillingAmount= 10,
             IsCostPerDay = false,
             HCPCBillingCode = "G0156"
         },
         new BillingCodeAndAmount()
         {
             BillingType= TotalChargeConstant,
             RevenueBillingCode = "0001",
             BillingAmount= 10,
             IsCostPerDay = false,
             HCPCBillingCode = ""
         },
         new BillingCodeAndAmount()
         {
             BillingType= rxConstant,
             RevenueBillingCode = "0250",
             BillingAmount= 12,
             IsCostPerDay = false,
             HCPCBillingCode = ""
         }

     };

    }

    public class BillingCodeAndAmount
    {
        public string BillingType { get; set; }
        public string RevenueBillingCode;
        public int BillingAmount;
        public bool IsCostPerDay { get; set; }
        public string HCPCBillingCode { get; set; }
    }
}
