using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using BDMPro.Models;
using BDMPro.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using BDMPro.Data;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Google.Protobuf.WellKnownTypes;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

namespace BDMPro.Utils
{
    public class Util : IDisposable
    {
        private DefaultDBContext db;
        private IConfiguration Configuration;
        private IWebHostEnvironment Environment;
        public Util(DefaultDBContext _db, IConfiguration _Configuration, IWebHostEnvironment _Environment)
        {
            db = _db;
            Configuration = _Configuration;
            Environment = _Environment;
        }

        public CurrentUserPermission GetCurrentUserPermission(string aspnetUserId, string moduleCode)
        {
            string moduleId = db.Modules.Where(a => a.Code == moduleCode).Select(a => a.Id).FirstOrDefault();

            CurrentUserPermission currentUserPermission = new CurrentUserPermission();
            List<string> roles = GetCurrentUserRoleIdList(aspnetUserId);

            List<RoleModulePermission> permissions = new List<RoleModulePermission>();
            permissions = (from t1 in db.RoleModulePermissions.AsEnumerable()
                           join t2 in roles on t1.RoleId equals t2
                           where t1.ModuleId == moduleId
                           select t1).ToList();
            if (permissions.Count == 0)
            {
                currentUserPermission.ViewRight = false;
                currentUserPermission.AddRight = false;
                currentUserPermission.EditRight = false;
                currentUserPermission.DeleteRight = false;
            }
            else
            {
                currentUserPermission.ViewRight = permissions.Any(a => a.ViewRight == true);
                currentUserPermission.AddRight = permissions.Any(a => a.AddRight == true);
                currentUserPermission.EditRight = permissions.Any(a => a.EditRight == true);
                currentUserPermission.DeleteRight = permissions.Any(a => a.DeleteRight == true);
            }

            return currentUserPermission;
        }

        public List<string> GetCurrentUserRoleIdList(string aspnetUserId)
        {
            List<string> roles = new List<string>();
            roles = db.AspNetUserRoles.Where(a => a.UserId == aspnetUserId).Select(a => a.RoleId).ToList();
            return roles;
        }

        public List<string> GetCurrentUserRoleNameList(string aspnetUserId)
        {
            List<string> nameList = new List<string>();
            List<string> roleIds = GetCurrentUserRoleIdList(aspnetUserId);
            foreach (var id in roleIds)
            {
                string name = db.AspNetRoles.Where(a => a.Id == id).Select(a => a.Name).FirstOrDefault();
                nameList.Add(name);
            }
            return nameList;
        }

        public CurrentUserPermission CanView(string aspnetUserId, string moduleName)
        {
            string moduleId = db.Modules.Where(a => a.Name == moduleName).Select(a => a.Id).FirstOrDefault();

            CurrentUserPermission currentUserPermission = new CurrentUserPermission();
            List<string> roles = GetCurrentUserRoleIdList(aspnetUserId);

            List<bool> permissions = new List<bool>();
            permissions = (from t1 in db.RoleModulePermissions
                           join t2 in roles on t1.RoleId equals t2
                           where t1.ModuleId == moduleId
                           select t1.ViewRight).ToList();
            if (permissions.Count == 0)
            {
                currentUserPermission.ViewRight = false;
            }
            else
            {
                currentUserPermission.ViewRight = permissions.Any(a => a == true);
            }

            return currentUserPermission;
        }

        public string GetGlobalOptionSetCode(string Id)
        {
            string id = db.GlobalOptionSets.Where(a => a.Id == Id && a.Status == "Active").Select(a => a.Code).FirstOrDefault();
            return id;
        }

        public bool ValidateImageFile(string fileName)
        {
            bool validated = false;
            if (fileName.Contains("jpg", StringComparison.OrdinalIgnoreCase) || fileName.Contains("jpeg", StringComparison.OrdinalIgnoreCase) || fileName.Contains("png", StringComparison.OrdinalIgnoreCase))
            {
                validated = true;
            }
            return validated;
        }

        public CreatedAndModifiedViewModel GetCreatedAndModified(string createdBy, string isoUtcCreatedOn, string modifiedBy, string isoUtcModifiedOn)
        {
            CreatedAndModifiedViewModel model = new CreatedAndModifiedViewModel();
            model.CreatedByName = db.UserProfiles.Where(a => a.AspNetUserId == createdBy).Select(a => a.FullName).FirstOrDefault();
            model.ModifiedByName = db.UserProfiles.Where(a => a.AspNetUserId == modifiedBy).Select(a => a.FullName).FirstOrDefault();
            model.FormattedCreatedOn = "";
            model.FormattedModifiedOn = "";
            if (string.IsNullOrEmpty(isoUtcCreatedOn) == false)
            {
                model.FormattedCreatedOn = isoUtcCreatedOn;
            }
            if (string.IsNullOrEmpty(isoUtcModifiedOn) == false)
            {
                model.FormattedModifiedOn = isoUtcModifiedOn;
            }
            return model;
        }

        public DateTime? GetSystemTimeZoneDateTimeNow()
        {
            string timeZone = Configuration["TimeZone"];
            DateTime dateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, TimeZoneInfo.Local.Id, timeZone);
            DateTime utcDt = DateTime.UtcNow;
            DateTime localDt = utcDt.ToLocalTime();
            return dateTime;
        }

        public string GetIsoUtcNow()
        {
            return DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
        }

        public DateTime? ConvertToSystemTimeZoneDateTime(string isoUtc)
        {
            DateTime dateTimeUtc = DateTime.Parse(isoUtc);
            string timeZone = Configuration["TimeZone"];
            DateTime result = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTimeUtc, TimeZoneInfo.Utc.Id, timeZone);
            return result;
        }

        public List<SelectListItem> GetGlobalOptionSets(string type, string selectedId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = (from t1 in db.GlobalOptionSets
                    where t1.Type == type && t1.Status == "Active"
                    orderby t1.OptionOrder
                    select new SelectListItem
                    {
                        Value = t1.Id,
                        Text = t1.DisplayName,
                        Selected = t1.Id == selectedId ? true : false
                    }).ToList();
            return list;
        }

        public void SaveUserAttachment(IFormFile file, string userProfileId, string attachmentTypeId, string uploadedById)
        {
            var fileNameWithExtension = Path.GetFileName(file.FileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            string shortUniqueId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            string uniqueid = StringExtensions.RemoveSpecialCharacters(shortUniqueId);
            fileNameWithoutExtension = fileNameWithoutExtension.Replace(" ", "");//remove space to form a valid url
            string uniqueFileName = fileNameWithoutExtension + "_" + uniqueid + extension;
            var path = Path.Combine(this.Environment.WebRootPath, "UploadedFiles", uniqueFileName);
            string relativePath = "\\" + Path.Combine("UploadedFiles", uniqueFileName);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            UserAttachment userAttachment = new UserAttachment();
            userAttachment.Id = Guid.NewGuid().ToString();
            userAttachment.UserProfileId = userProfileId;
            userAttachment.FileName = fileNameWithExtension;
            userAttachment.UniqueFileName = uniqueFileName;
            userAttachment.FileUrl = relativePath;
            userAttachment.AttachmentTypeId = attachmentTypeId;
            userAttachment.CreatedBy = uploadedById;
            userAttachment.CreatedOn = GetSystemTimeZoneDateTimeNow();
            userAttachment.IsoUtcCreatedOn = GetIsoUtcNow();
            db.UserAttachments.Add(userAttachment);
            db.SaveChanges();
        }

        public string GetGlobalOptionSetDisplayName(string Id)
        {
            string id = db.GlobalOptionSets.Where(a => a.Id == Id && a.Status == "Active").Select(a => a.DisplayName).FirstOrDefault();
            return id;
        }

        public void SaveLoginHistory(string userId)
        {
            LoginHistory loginHistory = new LoginHistory();
            loginHistory.Id = Guid.NewGuid().ToString();
            loginHistory.AspNetUserId = userId;
            loginHistory.LoginDateTime = GetSystemTimeZoneDateTimeNow();
            loginHistory.IsoUtcLoginDateTime = GetIsoUtcNow();
            db.LoginHistories.Add(loginHistory);
            db.SaveChanges();
        }

        public UserProfileViewModel GetCurrentUserProfile(string currentUserId)
        {
            UserProfileViewModel model = new UserProfileViewModel();
            string profilePicTypeId = GetGlobalOptionSetId(ProjectEnum.UserAttachment.ProfilePicture.ToString(), "UserAttachment");
            model = (from t1 in db.UserProfiles
                     join t2 in db.AspNetUsers on t1.AspNetUserId equals t2.Id
                     where t2.Id == currentUserId
                     select new UserProfileViewModel
                     {
                         Id = t1.Id,
                         AspNetUserId = t1.AspNetUserId,
                         FullName = t1.FullName,
                         IDCardNumber = t1.IDCardNumber,
                         FirstName = t1.FirstName,
                         LastName = t1.LastName,
                         DateOfBirth = t1.DateOfBirth,
                         PhoneNumber = t1.PhoneNumber,
                         Address = t1.Address,
                         PostalCode = t1.PostalCode,
                         Username = t2.UserName,
                         EmailAddress = t2.Email,
                         UserStatusId = t1.UserStatusId,
                         GenderId = t1.GenderId,
                         CountryName = t1.CountryName,
                         CreatedBy = t1.CreatedBy,
                         ModifiedBy = t1.ModifiedBy,
                         CreatedOn = t1.CreatedOn,
                         ModifiedOn = t1.ModifiedOn,
                         IsoUtcCreatedOn = t1.IsoUtcCreatedOn,
                         IsoUtcModifiedOn = t1.IsoUtcModifiedOn,
                         IsoUtcDateOfBirth = t1.IsoUtcDateOfBirth,
                         DateOfBirthIsoString = t1.IsoUtcDateOfBirth
                     }).FirstOrDefault();
            model.UserStatusName = db.GlobalOptionSets.Where(a => a.Id == model.UserStatusId).Select(a => a.DisplayName).FirstOrDefault();
            model.UserRoleIdList = (from t1 in db.AspNetUserRoles
                                    join t2 in db.AspNetRoles on t1.RoleId equals t2.Id
                                    where t1.UserId == model.AspNetUserId
                                    select t2.Name).ToList();
            model.UserRoleName = String.Join(", ", model.UserRoleIdList);
            model.GenderName = db.GlobalOptionSets.Where(a => a.Id == model.GenderId).Select(a => a.DisplayName).FirstOrDefault();
            model.ProfilePictureFileName = db.UserAttachments.Where(a => a.UserProfileId == model.Id && a.AttachmentTypeId == profilePicTypeId).OrderByDescending(o => o.CreatedOn).Select(a => a.UniqueFileName).FirstOrDefault();
            model.CreatedAndModified = GetCreatedAndModified(model.CreatedBy, model.IsoUtcCreatedOn, model.ModifiedBy, model.IsoUtcModifiedOn);
            model.IsoUtcDateOfBirth = model.IsoUtcDateOfBirth.Substring(0, 10);
            //model.DateOfBirth = model.DateOfBirth.Value.ToLocalTime();
            //model.DateOfBirthIsoString = model.IsoUtcDateOfBirth;
            //model.IsoUtcDateOfBirth = model.IsoUtcDateOfBirth;

            return model;
        }

        public List<SelectListItem> GetCountryList(string selectedName)
        {
            List<SelectListItem> countryList = new List<SelectListItem>();
            List<string> countries = db.Countries.Select(a => a.Name).ToList();
            foreach (string country in countries)
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = country;
                selectListItem.Value = country;
                selectListItem.Selected = selectedName == country ? true : false;
                countryList.Add(selectListItem);
            }
            return countryList.OrderBy(a => a.Text).ToList();
        }

        public string GetGlobalOptionSetId(string code, string type)
        {
            string id = db.GlobalOptionSets.Where(a => a.Code == code && a.Type == type && a.Status == "Active").Select(a => a.Id).FirstOrDefault();
            return id;
        }

        public List<SelectListItem> GetDataForDropDownList<T>(
            string selectedId,
            DbSet<T> dbSet,
            Expression<Func<T, string>> fieldNameForText,
            Expression<Func<T, string>> fieldNameForValue,
            Expression<Func<T, bool>> filter = null,
            Expression<Func<T, object>> orderBy = null,
            string order = "asc")
            where T : class
        {
            IQueryable<T> query = dbSet.AsQueryable();

            //filtering
            query = filter != null ? query.Where(filter) : query;

            //ordering
            query = orderBy != null ? (order == "asc" ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy)) : query.OrderBy(fieldNameForText);

            List<SelectListItem> list = query.Select(x => new SelectListItem
            {
                Text = fieldNameForText.Compile()(x),
                Value = fieldNameForValue.Compile()(x),
                Selected = (selectedId == fieldNameForValue.Compile()(x))
            }).ToList();

            return list;
        }

        public bool UsernameExists(string username, string currentRecordId)
        {
            bool usernameExist = false;
            if (string.IsNullOrEmpty(currentRecordId))
            {
                usernameExist = db.AspNetUsers.Where(a => a.UserName == username).Any();
            }
            else
            {
                usernameExist = db.AspNetUsers.Where(a => a.UserName == username && a.Id != currentRecordId).Any();
            }
            return usernameExist;
        }

        public bool EmailExists(string email, string currentRecordId)
        {
            bool emailExist = false;
            if (string.IsNullOrEmpty(currentRecordId))
            {
                emailExist = db.AspNetUsers.Where(a => a.Email == email).Any();
            }
            else
            {
                emailExist = db.AspNetUsers.Where(a => a.Email == email && a.Id != currentRecordId).Any();
            }
            return emailExist;
        }

        public string GetUserProfileId(string aspNetUserId)
        {
            string id = db.UserProfiles.Where(a => a.AspNetUserId == aspNetUserId).Select(a => a.Id).FirstOrDefault();
            return id;
        }

        public EmailTemplate EmailTemplateForConfirmEmail(string Username, string callbackUrl)
        {
            string websiteName = Configuration["PortalName"];
            EmailTemplate emailTemplate = db.EmailTemplates.Where(a => a.Type == "ConfirmEmail").FirstOrDefault();
            string subject = emailTemplate.Subject;
            string body = emailTemplate.Body;
            subject = ReplaceWords(subject, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[Username]", Username);
            body = ReplaceWords(body, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[Url]", callbackUrl);
            emailTemplate.Subject = subject;
            emailTemplate.Body = body;
            return emailTemplate;
        }

        public EmailTemplate EmailTemplateForPasswordResetByAdmin(string Username = "", string ResetByName = "", string NewPassword = "")
        {
            string websiteName = Configuration["PortalName"];
            EmailTemplate emailTemplate = db.EmailTemplates.Where(a => a.Type == ProjectEnum.EmailTemplate.PasswordResetByAdmin.ToString()).FirstOrDefault();
            string subject = emailTemplate.Subject;
            string body = emailTemplate.Body;
            subject = ReplaceWords(subject, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[Username]", Username);
            body = ReplaceWords(body, "[ResetByName]", ResetByName);
            body = ReplaceWords(body, "[NewPassword]", NewPassword);
            emailTemplate.Subject = subject;
            emailTemplate.Body = body;
            return emailTemplate;
        }

        public EmailTemplate EmailTemplateForForgotPassword(string Username, string callbackUrl)
        {
            string websiteName = Configuration["PortalName"];
            EmailTemplate emailTemplate = db.EmailTemplates.Where(a => a.Type == "ForgotPassword").FirstOrDefault();
            string subject = emailTemplate.Subject;
            string body = emailTemplate.Body;
            subject = ReplaceWords(subject, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[Username]", Username);
            body = ReplaceWords(body, "[WebsiteName]", websiteName);
            body = ReplaceWords(body, "[Url]", callbackUrl);
            emailTemplate.Subject = subject;
            emailTemplate.Body = body;
            return emailTemplate;
        }

        public string GetMyProfilePictureName(string userid)
        {
            string fileName = "";
            if (!string.IsNullOrEmpty(userid))
            {
                string upId = GetUserProfileId(userid);
                string profilePictureTypeId = GetGlobalOptionSetId(ProjectEnum.UserAttachment.ProfilePicture.ToString(), "UserAttachment");
                fileName = db.UserAttachments.Where(a => a.UserProfileId == upId && a.AttachmentTypeId == profilePictureTypeId).OrderByDescending(o => o.CreatedOn).Select(a => a.UniqueFileName).FirstOrDefault();
            }
            return fileName;
        }

        public string ReplaceWords(string sentence, string target, string replaceWith)
        {
            string result = sentence.Replace(target, replaceWith);
            return result;
        }

        public void SendEmail(string email, string subject, string body)
        {
            string host = Configuration["SmtpHost"];
            string strPort = Configuration["SmtpPort"];
            int port = Int32.Parse(strPort);
            string userName = Configuration["SmtpUserName"];
            string password = Configuration["SmtpPassword"];
            var client = new SmtpClient(host, port);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(userName, password);
            client.EnableSsl = true;

            MailMessage mail = new MailMessage(userName, email, subject, body);
            mail.IsBodyHtml = true;
            client.Send(mail);
        }

        public List<SelectListItem> GetRolesForMultiSelect(List<string> selectedRoleNames)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            if (selectedRoleNames != null)
            {
                list = (from t1 in db.AspNetRoles
                        orderby t1.Name
                        select new SelectListItem
                        {
                            Text = t1.Name,
                            Value = t1.Name,
                            Selected = selectedRoleNames.Contains(t1.Name) ? true : false
                        }).ToList();
            }
            else
            {
                list = (from t1 in db.AspNetRoles
                        orderby t1.Name
                        select new SelectListItem
                        {
                            Text = t1.Name,
                            Value = t1.Name
                        }).ToList();
            }
            return list;
        }
        public List<string> ValidateColumns(List<string> dtColumns, List<string> columns)
        {
            var errors = new List<string>();
            //check either the provided columns and required columns length same
            if (dtColumns.Count != columns.Count)
            {
                errors.Add(Resource.ColumnsCountMismatch);
            }
            else
            {
                foreach (var column in columns)
                {
                    if (!dtColumns.Contains(column))
                        errors.Add($"{Resource.Column} {column} {Resource.notfoundormismatch}");
                }
            }
            return errors;
        }

        public List<string> ValidateImportUserFromExcel(UserProfileViewModel model)
        {
            List<string> errors = new List<string>();
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.EmailAddress)
                    || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword)
                    || string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.CountryName))
                {
                    errors.Add(Resource.SomeRequiredFieldsAreEmpty);
                }
                if (model.Password != model.ConfirmPassword)
                {
                    errors.Add(Resource.PasswordNotMatch);
                }
                PasswordValidation passwordValidation = new PasswordValidation();
                if (passwordValidation.IsValid(model.Password) == false)
                {
                    errors.Add(passwordValidation.ErrorMessage);
                }
                string phonePattern = @"^\+?\d{1,4}?[-.\s]?\(?\d{1,3}?\)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}[-.\s]?\d{1,9}$";
                if (Regex.Match(model.PhoneNumber, phonePattern).Success == false)
                {
                    errors.Add(Resource.InvalidPhoneNumber);
                }
                EmailAddressAttribute emailAddressAttribute = new EmailAddressAttribute();
                bool emailValid = emailAddressAttribute.IsValid(model.EmailAddress);
                if (emailValid == false)
                {
                    errors.Add(Resource.InvalidEmailAddress);
                }
                string usernamePattern = @"^[A-Za-z]\w{3,29}$";
                if (Regex.Match(model.Username, usernamePattern).Success == false)
                {
                    errors.Add(Resource.InvalidUsername);
                }
                List<SelectListItem> countries = GetCountryList("");
                if (countries?.Where(a => a.Text == model.CountryName).Any() == false)
                {
                    errors.Add(Resource.CountryNotFound);
                }
                var _user = db.AspNetUsers.FirstOrDefault(a => a.UserName == model.Username || a.Email == model.EmailAddress);
                if (_user != null)
                {
                    errors.Add($"{Resource.User} {model.Username} {Resource.alreadyexists}.");
                }
            }
            return errors;
        }

        public List<string> ValidateImportSupplierFromExcel(SupplierViewModel model)
        {
            List<string> errors = new List<string>();
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.SupplierName) || 
                    string.IsNullOrEmpty(model.EmailAddress) || 
                    string.IsNullOrEmpty(model.PhoneNumber) || 
                    string.IsNullOrEmpty(model.Address) || 
                    string.IsNullOrEmpty(model.ContactId))
                {
                    errors.Add(Resource.SomeRequiredFieldsAreEmpty);
                }
                string phonePattern = @"^\+?\d{1,4}?[-.\s]?\(?\d{1,3}?\)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}[-.\s]?\d{1,9}$";
                if (Regex.Match(model.PhoneNumber, phonePattern).Success == false)
                {
                    errors.Add(Resource.InvalidPhoneNumber);
                }
                EmailAddressAttribute emailAddressAttribute = new EmailAddressAttribute();
                bool emailValid = emailAddressAttribute.IsValid(model.EmailAddress);
                if (emailValid == false)
                {
                    errors.Add(Resource.InvalidEmailAddress);
                }
                string suppliernamePattern = @"^[A-Za-z]\w{3,29}$";
                if (Regex.Match(model.SupplierName, suppliernamePattern).Success == false)
                {
                    errors.Add(Resource.InvalidSuppliername);
                }
                string addressPattern = @"^[A-Za-z]\w{3,29}$";
                if (Regex.Match(model.Address, addressPattern).Success == false)
                {
                    errors.Add(Resource.InvalidAddress);
                }
                var _user = db.Suppliers.FirstOrDefault(a => a.SupplierName == model.SupplierName || a.Email == model.EmailAddress);
                if (_user != null)
                {
                    errors.Add($"{Resource.User} {model.SupplierName} {Resource.alreadyexists}.");
                }
            }
            return errors;
        }

        #region Excel


        public WorksheetPart GetWorksheetPart(WorkbookPart workbookPart, string sheetName)
        {
            string relId = workbookPart.Workbook.Descendants<Sheet>().First(s => sheetName.Equals(s.Name)).Id;
            return (WorksheetPart)workbookPart.GetPartById(relId);
        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        public int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        public Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }
        public byte[] CreateDropDownListValueInExcel(string path, string tempFilePath, List<string> valuesToInsert, string excelSheetName)
        {
            if (System.IO.File.Exists(path))
            {
                Workbook wb = new Workbook();

                //clone the question excel template
                byte[] byteArray = System.IO.File.ReadAllBytes(path);
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(byteArray, 0, (int)byteArray.Length);
                    System.IO.File.WriteAllBytes(tempFilePath, stream.ToArray());
                }

                // Open cloned excel document
                using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Open(tempFilePath, true))
                {
                    // Get the SharedStringTablePart. If it does not exist, create a new one.
                    SharedStringTablePart shareStringPart;
                    if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                    {
                        shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                    }
                    else
                    {
                        shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
                    }

                    //get the hidden Subject sheet
                    WorksheetPart worksheetPart = GetWorksheetPart(spreadSheet.WorkbookPart, excelSheetName);

                    uint count = 1;

                    //insert subject name inside the hidden Subject sheet
                    foreach (var value in valuesToInsert)
                    {
                        int index = InsertSharedStringItem(value, shareStringPart);
                        Cell cell = InsertCellInWorksheet("A", count, worksheetPart);
                        cell.CellValue = new CellValue(index.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                        worksheetPart.Worksheet.Save();
                        count++;
                    }
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(tempFilePath);

                //delete the cloned excel file from device
                System.IO.File.Delete(tempFilePath);

                return fileBytes;
            }

            return null;
        }

        #endregion

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }
    }
}
