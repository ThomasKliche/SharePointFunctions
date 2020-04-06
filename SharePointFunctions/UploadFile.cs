using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using Microsoft.SharePoint.Client;
using System.IO;

namespace SharePointFunctions
{
    public class UploadFile
    {
        public string SiteUrl { get; set; }
        public string DocumentLibrary { get; set; }
        public string FileName { get; set; }
        public string Folder { get; set; }
        public string UserName { get; set; }
        public bool OverWrite { get; set; } = false;

        private SecureString SecPassword;
        public string Password {
            set
            {
                SecureString securePassword = new SecureString();
                foreach (char c in value)
                { securePassword.AppendChar(c); }
                this.SecPassword = securePassword;
            }
        }


        public void StartUpload()
        {
            UploadFileToSharePoint();
        }

        private void UploadFileToSharePoint()
        {
            SharePointOnlineCredentials onlineCredentials = new SharePointOnlineCredentials(this.UserName, this.SecPassword);
            using (ClientContext CContext = new ClientContext(SiteUrl))
            {
                CContext.Credentials = onlineCredentials;
                Web web = CContext.Web;
                FileCreationInformation newFile = new FileCreationInformation();
                byte[] FileContent = System.IO.File.ReadAllBytes(this.FileName);
                newFile.ContentStream = new MemoryStream(FileContent);
                newFile.Url = Path.GetFileName(this.FileName);
                newFile.Overwrite = this.OverWrite;
                List DocumentLibrary = web.Lists.GetByTitle(this.DocumentLibrary);
                //SP.Folder folder = DocumentLibrary.RootFolder.Folders.GetByUrl(ClientSubFolder);
                Folder Clientfolder = DocumentLibrary.RootFolder.Folders.Add(this.Folder);
                Clientfolder.Update();
                Microsoft.SharePoint.Client.File uploadFile = Clientfolder.Files.Add(newFile);

                CContext.Load(DocumentLibrary);
                CContext.Load(uploadFile);
                CContext.ExecuteQuery();
            }
        }
    }
}
