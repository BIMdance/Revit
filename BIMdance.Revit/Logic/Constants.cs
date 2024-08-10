// ReSharper disable InconsistentNaming

namespace BIMdance.Revit.Logic;

public static class Constants
{
    public const string AppPrefix = "APP"; // TODO
    public static class Lookup
    {
        public static class ColumnType
        {
            public const string Other             = "##OTHER##GENERAL";
            public const string LengthMillimeters = "##LENGTH##MILLIMETERS";
            public const string Number            = "##NUMBER##GENERAL";
        }
    }
    
    public static class SharedParameters
    {
        public static readonly Guid CableTrayConduitIds = new("1F52CCB0-5ED5-43D9-BCAD-9C0B6A223A86");
        public static readonly Guid ElectricalSystemIds = new("A5A1274E-4CDE-49FA-86EB-9EEF2221D7D6");
        public static readonly Guid ServiceType         = new("38A75B2D-9F32-41C8-86D6-B84D9C8BAE0B");
        
        public static class Cable
        {
            public static readonly Guid Count                   = new("02605D3C-419C-4860-8D2C-04D6A9D4AF2F");
            public static readonly Guid Designation             = new("0B7106D7-795F-4981-9AEC-AE1EC60D3F9B");
            public static readonly Guid Diameter                = new("331FF9B2-57ED-4933-BB32-34D218EF81DD");
            public static readonly Guid Length                  = new("315BC87E-941A-489B-ACFC-77C6E591EB6F");
            public static readonly Guid Length_Max              = new("C5305554-0DA1-4E1D-96F6-338C91528CEA");
            public static readonly Guid Length_InCableTray      = new("2C69AE7F-76DD-4BA8-840A-F48A28D33EF4");
            public static readonly Guid Length_OutsideCableTray = new("158A1C82-C5D6-427E-B828-C0DD51E4B405");
            public static readonly Guid Reserve                 = new("9A093637-EA68-4920-91D1-4ED6C4B1C9F3");
            public static readonly Guid Trace                   = new("3C465675-D0FA-4D5F-A997-77E21D61CA4D");
        }

        public static class CableTray
        {
            public static readonly Guid Filling        = new("B387334B-95F4-475A-8FD5-3FFD56556F2F");
            public static readonly Guid FillingPercent = new("8E6494C3-AAC9-4FAA-B3CA-7950322CA990");
        }

        public static class Circuit
        {
            public static readonly Guid Designation = new("20B6E044-AA6D-42BB-A30B-6916EC42CA51");
            public static readonly Guid Topology    = new("60F4E268-499F-4451-A6E2-0193F19523A0");
        }
        
        public static class ADSK
        {
            public static readonly Guid Count          = new("8d057bb3-6ccd-4655-9165-55526691fe3a");
            public static readonly Guid Description    = new("e6e0f5cd-3e26-485b-9342-23882b20eb43");
            public static readonly Guid Floor          = new("9eabf56c-a6cd-4b5c-a9d0-e9223e19ea3f");
            public static readonly Guid Group          = new("3de5f1a4-d560-4fa8-a74f-25d250fb3401");
            public static readonly Guid Manufacturer   = new("a8cdbf7b-d60a-485e-a520-447d2055f351");
            public static readonly Guid PartNr         = new("2fd9e8cb-84f3-4297-b8b8-75f444e124ed");
            public static readonly Guid Type           = new("2204049c-d557-4dfc-8d70-13f19715e46d");
            public static readonly Guid Units          = new("4289cb19-9517-45de-9c02-5a74ebf5c86d");
            public static readonly Guid Version_Family = new("85cd0032-c9ee-4cd3-8ffa-b2f1a05328e3");
            public static readonly Guid Version_Revit  = new("37384649-c3c8-4fc2-a08e-c2206438f528");
            public static readonly Guid Weight         = new("32989501-0d17-4916-8777-da950841c6d7");
            public static readonly Guid Weight_Element = new("5913a1f9-0b38-4364-96fe-a6f3cb7fcc68");
            
            public static class Size
            {
                public static readonly Guid Height    = new("da753fe3-ecfa-465b-9a2c-02f55d0c2ff1");
                public static readonly Guid Length    = new("748a2515-4cc9-4b74-9a69-339a8d65a212");
                public static readonly Guid Offset    = new("515dc061-93ce-40e4-859a-e29224d80a10");
                public static readonly Guid Thickness = new("293f055d-6939-4611-87b7-9a50d0c1f50e");
                public static readonly Guid Width     = new("8f2e4f93-9472-4941-a65d-0ac468fd6a5d");
            }
        }
    }
}