using System;
namespace hmsapi.Data
{
	public class col_mst_patient
	{
            public DateTime creation_date { get; set; }
            public string? id { get; set; }
            public string? title { get; set; }
            public string? patient_name { get; set; }
            public string? address { get; set; }
            public string? state { get; set; }
            public string? city { get; set; }
            public string? pincode { get; set; }
            public string? document_id { get; set; }
            public string? document_type { get; set; }
            public string? mobile { get; set; }
            public string? alternate_mobile { get; set; }
            public string? email { get; set; }
            public DateOnly dob { get; set; }
            public string? occupation { get; set; }
            public string? religion { get; set; }
            public string? mstatus { get; set; }
            public string? gender { get; set; }
            public string? emergency_contact { get; set; }
            public string? emergency_contact_relation { get; set; }
            public string? emergency_contact_mob_number { get; set; }
	}

    public class mst_patient
    {

    }
}

