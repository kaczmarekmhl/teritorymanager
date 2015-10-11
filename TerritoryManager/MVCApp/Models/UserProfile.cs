﻿using MVCApp.Translate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace MVCApp.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        #region Properties

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        public int UserId { get; set; }

        [Display(ResourceType = typeof(Strings), Name = "AccountUserName")]
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Display(ResourceType = typeof(Strings), Name = "UserFullName")]
        public string FullName
        {
            get
            {
                return string.Join(" ", FirstName, LastName);
            }
        }

        /// <summary>
        /// The congregation ID of UserProfile.
        /// </summary>
        public int? CongregationId { get; set; }

        /// <summary>
        /// Congregation that this user belongs to.
        /// </summary>
        [Display(ResourceType = typeof(Strings), Name = "AccountCongregation")]
        public virtual Congregation Congregation { get; set; }

        #endregion

        private SimpleRoleProvider roles = (SimpleRoleProvider)Roles.Provider;

        /// <summary>
        ///     Returns user roles.
        /// </summary>
        /// <returns>
        ///     List of user roles.
        /// </returns>
        public List<String> GetRoles()
        {
            return roles.GetRolesForUser(UserName).ToList<String>();
        }

        /// <summary>
        ///     Adds user to role.
        /// </summary>
        /// <param name="role">Role name.</param>
        public void AddToRole(SystemRoles role)
        {
            roles.AddUsersToRoles(new string[] { UserName }, new string[] { role.ToString() });
        }

        /// <summary>
        ///     Removes user from role.
        /// </summary>
        /// <param name="role">Role name</param>
        public void RemoveUserFromRole(SystemRoles role)
        {
            roles.RemoveUsersFromRoles(new string[] { UserName }, new string[] { role.ToString() });
        }
    }
}