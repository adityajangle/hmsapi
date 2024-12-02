using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using hmsapi.Services;
using Spire.Doc;
using System.Data;
using System.Text;

namespace hmsapi.Data
{
    public class col_mst_consent
    {
        public string? mst_consent_id { get; set; }
        public string? type { get; set; }
        public string? name { get; set; }
        public byte[]? template { get; set; }
    }
    public class col_pat_consent
    {
        public string? consent_id { get; set; }
        public string? patient_id { get; set; }
        public string? mst_consent_id { get; set; }
        public string? department_id { get; set; }
        public string? interpreted_by { get; set; }
        public string? doctor_id { get; set; }
        public string? witness_one_name { get; set; }
        public string? witness_one_rel { get; set; }
        public string? witness_two_name { get; set; }
        public string? witness_two_rel { get; set; }
        public DateTime? created_date { get; set; }
       // public TimeOnly created_time { get; set; }
        public string? created_by { get; set; }
        public string? consent_qna { get; set; }
    }
    public class mst_consent
    {
        public static List<col_mst_consent?> GetAllConsentType(IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from mst_consent", null);
            List<col_mst_consent?> consentList = new List<col_mst_consent?>();
            foreach (DataRow x in dtb.Rows)
            {
                col_mst_consent? consent = new col_mst_consent();
                consent = _dbOperations.SelectParsedData(consent, x);
                consentList.Add(consent);
            }
            return consentList;
        }
        public static void CreateConsent(IDbOperations dbOperations, col_pat_consent col)
        {
            List<string> _columns = new List<string>();
            col.GetType().GetProperties().ToList().ForEach(
                x => _columns.Add(x.Name));
            _columns.Remove("consent_id");
            col.created_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //_columns.Remove("created_date");//util package
            //_columns.Remove("created_time");
            string query = $"insert into pat_consent({string.Join(',', _columns.ToArray())})values( {string.Join(',', _columns.Select(x => $"@{x}").ToArray())})";
            dbOperations.ExecuteNonTransaction((command) => {
                dbOperations.AddCommandParams(col, command);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new DataException("Unable to add consent for patient");
                }
            }, query);
        }

        public static List<col_pat_consent?> GetAllPat_Consent(string patient_id,IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from pat_consent where patient_id={patient_id}", null);
            List<col_pat_consent?> pat_consentList = new List<col_pat_consent?>();
            foreach (DataRow x in dtb.Rows)
            {
                col_pat_consent? pat_consent = new col_pat_consent();
                pat_consent = _dbOperations.SelectParsedData(pat_consent, x);
                pat_consentList.Add(pat_consent);
            }
            return pat_consentList;
        }

        public static List<col_pat_consent?> GetAllPatConsentByAppointment(string patient_id,string appointment_id, IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select * from pat_consent where patient_id={patient_id} and appointment_id={appointment_id}", null);
            List<col_pat_consent?> pat_consentList = new List<col_pat_consent?>();
            foreach (DataRow x in dtb.Rows)
            {
                col_pat_consent? pat_consent = new col_pat_consent();
                pat_consent = _dbOperations.SelectParsedData(pat_consent, x);
                pat_consentList.Add(pat_consent);
            }
            return pat_consentList;
        }

        public static string GetConsentForm(string consent_id,IDbOperations _dbOperations)
        {
            DataTable dtb = _dbOperations.ExecuteTable($"select c.template from pat_consent pc join mst_consent c on c.mst_consent_id =pc.mst_consent_id where pc.consent_id={consent_id}", null);
            string template = "";
            if(dtb != null && dtb.Rows.Count>0 )
            {
                foreach (DataRow x in dtb.Rows) {
                    template = Encoding.UTF8.GetString((byte[]) x["template"]);
                }
            }
            // Get the base directory of the project
            string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;

            // Define the ConsentForm folder path
            string consentFormFolder = Path.Combine(projectRoot, "ConsentForm");

            // Ensure the folder exists
            Directory.CreateDirectory(consentFormFolder);

            // Define the full path for the file
            string filePath = Path.Combine(consentFormFolder, "output.docx");
            string outputpth = Path.Combine(consentFormFolder, "updated.docx");
            // Decode the Base64 string into a byte array
            byte[] fileBytes = Convert.FromBase64String(template);

            // Write the byte array to the file
            File.WriteAllBytes(filePath, fileBytes);
            
           

           string base64= CreateForm(filePath, outputpth,consent_id,_dbOperations);

           DeleteFile(filePath);
            //DeleteFile(outputpth);


            return base64;
        }

       
        public static string CreateForm(string inputPath, string outputPath, string consent_id,IDbOperations dbOperations)
        {
            DataTable dtb = dbOperations.ExecuteTable($"select u.user_name,p.patient_name,p.mobile,pc.created_date from pat_consent pc join mst_user u on u.id=pc.doctor_id join mst_patient p on p.id=pc.patient_id where pc.consent_id={consent_id}", null);
            string patient_name = "";
            string doctor_name = "";
            string created_date = "";
            string mobile = "";
            if (dtb != null && dtb.Rows.Count > 0)
            {
                foreach (DataRow x in dtb.Rows)
                {
                    patient_name = x["patient_name"].ToString()!;
                    doctor_name = x["user_name"].ToString()!;
                    created_date = Convert.ToDateTime(x["created_date"]).ToString("dd-MM-yyyy HH:mm:ss");
                    mobile = x["mobile"].ToString()!;
                }
            }


            File.Copy(inputPath, outputPath, true);

            // Open the copied .docx file for editing
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(outputPath, true))
            {
                // Access the main document part
                DocumentFormat.OpenXml.Wordprocessing.Document doc = wordDoc.MainDocumentPart!.Document;

                // Replace placeholders with actual values
                doc= ReplacePlaceholder(doc, "dr_name", doctor_name);
                doc= ReplacePlaceholder(doc, "patient_name", patient_name);
                doc = ReplacePlaceholder(doc,"created_date",created_date);
                doc = ReplacePlaceholder(doc, "mobile_number", mobile);
                doc = ReplacePlaceholder(doc, "{{signature}}", " ");

                // Save the changes
                doc.Save();
            }
            string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
            string consentFormFolder = Path.Combine(projectRoot, "ConsentForm");

            // Ensure the folder exists
            Directory.CreateDirectory(consentFormFolder);

            // Define the full path for the file
            string pdfPath = Path.Combine(consentFormFolder, "output.pdf");
            // Load the .docx file
            Spire.Doc.Document document = new Spire.Doc.Document();
            document.LoadFromFile(outputPath);
            document.EmbedFontsInFile= true;
            // Save the document as a .pdf file
            document.SaveToFile(pdfPath, FileFormat.PDF);
            // Read the PDF file as a byte array
            byte[] fileBytes = File.ReadAllBytes(pdfPath);

            // Convert the byte array to a Base64 string
            string base64 = Convert.ToBase64String(fileBytes);

            //DeleteFile(pdfPath);
            return base64;

           // Console.WriteLine($"PDF file created successfully at {pdfPath}");
        }
        static DocumentFormat.OpenXml.Wordprocessing.Document ReplacePlaceholder(DocumentFormat.OpenXml.Wordprocessing.Document doc, string placeholder, string replacement)
        {
            // Iterate through all text elements in the document
            foreach (var text in doc.Descendants<Text>())
            {
                if (text.Text.Contains(placeholder))
                {
                    text.Text = text.Text.Replace(placeholder, replacement);
                }
            }
            doc.Save();
            return doc;
        }

        static void DeleteFile(string filePath)
        {
            // Check if the file exists before attempting to delete
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
        }

        public static void AddConsent(IDbOperations _dbOperations)
        {

            string base64str = "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";
            string name = "Covid-19";
            string type = "Covid-19 Marathi Form";
            col_mst_consent col = new col_mst_consent();
            col.mst_consent_id = "2";
            col.type = type;
            col.name = name;
            col.template = Encoding.UTF8.GetBytes(base64str);

            List<string> _columns = new List<string>();
            col.GetType().GetProperties().ToList().ForEach(
                x => _columns.Add(x.Name));
            _columns.Remove("mst_consent_id");
           // col.created_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //_columns.Remove("created_date");//util package
            //_columns.Remove("created_time");
            string query = $"insert into mst_consent({string.Join(',', _columns.ToArray())})values( {string.Join(',', _columns.Select(x => $"@{x}").ToArray())})";
            _dbOperations.ExecuteNonTransaction((command) => {
                _dbOperations.AddCommandParams(col, command);
                if (command.ExecuteNonQuery() == 0)
                {
                    throw new DataException("Unable to add consent for patient");
                }
            }, query);

        }
    }
}
