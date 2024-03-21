using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace BDMPro.Models
{
    //project enum mặc định

    public class ProjectEnum
    {

        public enum ModuleCode
        {
            Dashboard,
            UserStatus,
            UserAttachmentType,
            RoleManagement,
            UserManagement,
            SupplierManagement,
            DeviceManagement,
            DeviceTypeManagement,
            RepairManagement,
            RepairTypeManagement,
            Statistical,
            LoginHistory
        }

        public enum UserAttachment
        {
            ProfilePicture
        }

        public enum Gender
        {
            Female,
            Male,
            Other
        }

        public enum UserStatus
        {
            Registered,
            Validated,
            NotValidated,
            Banned
        }

                public enum SupplierStatus
        {

        }

        public enum EmailTemplate
        {
            ConfirmEmail,
            PasswordResetByAdmin
        }

    }

}