﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dover.Framework.DAO;
using Dover.Framework.Monad;

namespace Dover.Framework.Service
{

    public enum Permission
    {
        Inactive,
        Active,
        Default
    }

    public class PermissionManager
    {

        private PermissionDAO permissionDAO;

        public PermissionManager(PermissionDAO permissionDAO)
        {
            this.permissionDAO = permissionDAO;
        }

        internal bool AddInEnabled(string addInName)
        {
            Permission perm;

            perm = permissionDAO.GetUserPermission(addInName);
            if (perm == Permission.Inactive)
                return false;
            else if (perm == Permission.Active)
                return true;
            else if (perm == Permission.Default)
            {
                perm = permissionDAO.GetAddInPermission(addInName);
                return (perm == Permission.Active);
            }

            return false;
        }

        internal void ConfigureAddIn(string addInName, Permission permission)
        {
            permissionDAO.SaveAddInPermission(addInName, permission);
        }

        internal Permission ParsePermissionStr(string permissionStr)
        {
            switch (permissionStr.Return(x => x, "A"))
            {
                case "A":
                    return Permission.Active;
                case "I":
                    return Permission.Inactive;
                default:
                    return Permission.Default;
            }
        }

        internal void ConfigureAddIn(string addInName, string userName, Permission permission)
        {
            string userPermissionCode = permissionDAO.GetUserPermissionCode(addInName, userName);
            if (userPermissionCode == null)
                permissionDAO.SaveAddInPermission(addInName, userName, permission);
            else
                permissionDAO.UpdateAddInPermission(userPermissionCode, permission);
        }

    }
}
