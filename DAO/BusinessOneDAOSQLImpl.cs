﻿/*
 *  Dover Framework - OpenSource Development framework for SAP Business One
 *  Copyright (C) 2014  Eduardo Piva
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *  
 *  Contact me at <efpiva@gmail.com>
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbobsCOM;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Castle.Core.Logging;
using System.Reflection;
using Dover.Framework.Monad;
using Dover.Framework.Model.SAP;
using Dover.Framework.Model;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Xml.Linq;
using Dover.Framework.Log;

namespace Dover.Framework.DAO
{

    public class BusinessOneDAOSQLImpl : BusinessOneDAO
    {
        private Company company;

        public ILogger Logger { get; set; }

        public BusinessOneDAOSQLImpl(Company company)
        {
            this.company = company;
        }

        public void SaveBOM(IBOM bom, INotifier notifier = null)
        {
            UpdateOrSaveBOMIfNotExists(bom, false, false, notifier);
        }

        public void SaveBOMIfNotExists(IBOM bom, INotifier notifier = null)
        {
            UpdateOrSaveBOMIfNotExists(bom, false, true, notifier);
        }

        public void UpdateOrSaveBOMIfNotExists(IBOM bom, INotifier notifier = null)
        {
            UpdateOrSaveBOMIfNotExists(bom, true, true, notifier);
        }

        private void UpdateOrSaveBOMIfNotExists(IBOM bom, bool Update = true, bool CheckExists = true, INotifier notifier = null)
        {            
            object obj = null;
            string xmlBom = bom.Serialize();

            company.XmlExportType = BoXmlExportTypes.xet_ExportImportMode;
            company.XMLAsString = true;

            int length = company.GetXMLelementCount(xmlBom);
            for (int i = 0; i < length; i++)
            {
                Logger.Debug(DebugString.Format(Messages.StartUpdateOrSave, bom.BO[i].GetName()));
                Type type = bom.BO[i].GetBOClassType();
                try
                {
                    obj = company.GetBusinessObject(bom.BO[i].GetBOType());
                    if (CheckExists)
                    {
                        var browser = type.InvokeMember("Browser", BindingFlags.GetProperty | BindingFlags.Public, null, obj, null);
                        browser.GetType().InvokeMember("ReadXml", BindingFlags.InvokeMethod | BindingFlags.Public,
                            null, browser, new object[] { xmlBom, i });
                        string xml = (string)type.InvokeMember("GetAsXML", BindingFlags.InvokeMethod | BindingFlags.Public, null, obj, null);
                        var resourceXml = XDocument.Parse(xml);
                        object[] keys = GetKeys(obj, bom.BO[i].GetBOType(), bom.BO[i].GetKey());
                        bool found = (bool)type.InvokeMember("GetByKey", BindingFlags.InvokeMethod | BindingFlags.Public, null, obj, keys);
                        if (found)
                        {
                            if (Update)
                                UpdateDIObject(obj, xml, resourceXml, bom.BO[i].GetName(), bom.BO[i].GetFormattedKey(), notifier);
                        }
                        else
                        {
                            AddDIObject(obj, xmlBom, i, bom.BO[i].GetName(), bom.BO[i].GetFormattedKey(), notifier);
                        }
                    }
                    else
                    {
                        AddDIObject(obj, xmlBom, i, bom.BO[i].GetName(), bom.BO[i].GetFormattedKey(), notifier);
                    }
                    
                }
                finally
                {
                    if (obj != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                }
                Logger.Debug(DebugString.Format(Messages.EndUpdateOrSave, bom.BO[i].GetName()));
            }
        }

        public List<int> ListOutdatedBOMKeys(IBOM bom)
        {
            return ListMissingOrOutdatedKeysBOM(bom, true);
        }

        public List<int> ListMissingBOMKeys(IBOM bom)
        {
            return ListMissingOrOutdatedKeysBOM(bom);
        }

        private List<int> ListMissingOrOutdatedKeysBOM(IBOM bom, bool update = false)
        {
            object obj = null;
            List<int> missingKeys = new List<int>();

            string xmlBom = bom.Serialize();

            company.XmlExportType = BoXmlExportTypes.xet_ExportImportMode;
            company.XMLAsString = true;

            int length = company.GetXMLelementCount(xmlBom);
            for (int i = 0; i < length; i++)
            {
                Logger.Debug(DebugString.Format(Messages.StartListMissingBOMKeys, bom.BO[i].GetName()));
                Type type = bom.BO[i].GetBOClassType();
                try
                {
                    obj = company.GetBusinessObject(bom.BO[i].GetBOType());
                    var browser = type.InvokeMember("Browser", BindingFlags.GetProperty | BindingFlags.Public, null, obj, null);
                    browser.GetType().InvokeMember("ReadXml", BindingFlags.InvokeMethod | BindingFlags.Public,
                        null, browser, new object[] { xmlBom, i });
                    string xml = (string)type.InvokeMember("GetAsXML", BindingFlags.InvokeMethod | BindingFlags.Public, null, obj, null);
                    var resourceXml = XDocument.Parse(xml);
                    object[] keys = GetKeys(obj, bom.BO[i].GetBOType(), bom.BO[i].GetKey());
                    bool found = (bool)type.InvokeMember("GetByKey", BindingFlags.InvokeMethod | BindingFlags.Public, null, obj, keys);
                    if (!found)
                    {
                        missingKeys.Add(i);
                    }
                    else if (update)
                    {
                        xml = (string)type.InvokeMember("GetAsXML", BindingFlags.InvokeMethod | BindingFlags.Public, null, obj, null);
                        var currXml = XDocument.Parse(xml);
                        if (!XDocument.DeepEquals(currXml, resourceXml))
                        {
                            missingKeys.Add(i);
                        }
                    }
                }
                finally
                {
                    if (obj != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                }
                Logger.Debug(DebugString.Format(Messages.EndListMissingBOMKeys, bom.BO[i].GetName()));
            }
            return missingKeys;
        }


        private void AddDIObject<T>(T obj, string xmlBom, int i, string name, string formatName, INotifier notifier)
        {
            Type type = typeof(T);
            var browser = type.InvokeMember("Browser", BindingFlags.GetProperty | BindingFlags.Public, null, obj, null);
            browser.GetType().InvokeMember("ReadXml", BindingFlags.InvokeMethod | BindingFlags.Public,
                null, browser, new object[] { xmlBom, i });
            int ret = (int)type.InvokeMember("Add", BindingFlags.InvokeMethod | BindingFlags.Public, null, obj, null);
            if (ret != 0)
            {
                string err;
                company.GetLastError(out ret, out err);
                string exceptionErr = String.Format(Messages.ErrorAddingDI, name, formatName, err);
                Logger.Error(exceptionErr);
                notifier.Do(x => x.OnError(xmlBom, ret, err));
                throw new Exception(exceptionErr);
            }
            else
            {
                string key = company.GetNewObjectKey();
                Logger.Info(String.Format(Messages.SuccessAddindDI, name, formatName));
                notifier.Do(x => x.OnSuccess(xmlBom, key));
            }
        }

        private void UpdateDIObject<T>(T obj, string resourceXmlString, XDocument resourceXml, string name, string formatName, INotifier notifier)
        {
            Type type = typeof(T);
            string xml = (string)type.InvokeMember("GetAsXML", BindingFlags.InvokeMethod | BindingFlags.Public, null, obj, null);
            var currXml = XDocument.Parse(xml);
            if (!XDocument.DeepEquals(currXml, resourceXml))
            {
                var browser = type.InvokeMember("Browser", BindingFlags.GetProperty | BindingFlags.Public, null, obj, null);
                browser.GetType().InvokeMember("ReadXml", BindingFlags.InvokeMethod | BindingFlags.Public,
                    null, browser, new object[] { resourceXmlString, 0 });
                int ret = (int)type.InvokeMember("Update", BindingFlags.InvokeMethod | BindingFlags.Public, null, obj, null);
                if (ret != 0)
                {
                    string err;
                    company.GetLastError(out ret, out err);
                    string exceptionErr = String.Format(Messages.ErrorUpdatingDI, name, formatName, err);
                    Logger.Error(exceptionErr);
                    notifier.Do(x => x.OnError(resourceXml.ToString(), ret, err));
                    throw new Exception(exceptionErr);
                }
                else
                {
                    Logger.Info(String.Format(Messages.SuccessUpdatingDI, name, formatName));
                    notifier.Do(x => x.OnSuccess(resourceXml.ToString(), ""));
                }
            }
            else
            {
                Logger.Debug(DebugString.Format(Messages.UpdateDINotNecessary, name));
            }
        }

        private object[] GetKeys<T>(T obj, BoObjectTypes objType, string[] args)
        {
            Type type = typeof(T);
            object[] ret;

            ret = new object[args.Length];
            for (int i=0 ; i < args.Length ; i++)
            {
                ret[i] = type.InvokeMember(args[i],
                    BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public, null, obj, null);
            }

            if (objType == BoObjectTypes.oUserFields)
            {

                ret[1] = ExecuteSqlForObject<string>(
                    String.Format(this.GetSQL("GetCUFDKey.sql"), ret[0], ret[1]));
                if (ret[1] == null)
                    ret[1] = "-1";
            }

            return ret;
        }

        public string GetNextCode(string udt)
        {
            SAPbobsCOM.Recordset objRecordSet = (SAPbobsCOM.Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            Logger.Debug(DebugString.Format(Messages.GetNextCodeStart, udt));
            string id;
            try
            {
                String query = String.Format(this.GetSQL("GetNextCode.sql"), udt);
                objRecordSet.DoQuery(query);

                if (objRecordSet.Fields.Item(0).Value.ToString() == "")
                {
                    id = "00000001";
                }
                else
                {
                    bool dbo_IncreaseCode;
                    int din_MyValue;
                    string dst_MyNewCode, pst_Code;
                    char dst_Caracter;

                    pst_Code = objRecordSet.Fields.Item(0).Value.ToString();

                    dst_MyNewCode = "";
                    dbo_IncreaseCode = true;
                    for (int i = 0; i <= 7; i++)
                    {
                        if (dbo_IncreaseCode == true)
                        {
                            dst_Caracter = pst_Code[7 - i];
                            din_MyValue = Convert.ToInt32(dst_Caracter);
                            if ((din_MyValue >= 48) && (din_MyValue < 57))  //the value ascii code is between 0 and 9, so it should be incresed by 1
                            {
                                dbo_IncreaseCode = false;
                                dst_MyNewCode = (char)(din_MyValue + 1) + dst_MyNewCode;
                            }
                            else if (din_MyValue == 57) //the value is 9, the next one would be A
                            {
                                dbo_IncreaseCode = false;
                                dst_MyNewCode = (char)(65) + dst_MyNewCode;
                            }
                            else if ((din_MyValue >= 65) && (din_MyValue < 90))  //The ascii code is between A and Z
                            {
                                dbo_IncreaseCode = false;
                                dst_MyNewCode = (char)(din_MyValue + 1) + dst_MyNewCode;
                            }
                            else if (din_MyValue == 90)     // the value is Z, It has to come back to 1, and inceease the next digit
                            {
                                dbo_IncreaseCode = true;    // this allow to increse the next digit
                                dst_MyNewCode = (char)(48) + dst_MyNewCode;
                            }
                        }
                        else
                        {
                            dst_MyNewCode = pst_Code.Substring(7 - i, 1) + dst_MyNewCode;
                        }
                    }

                    id = dst_MyNewCode;
                }
            }
            finally
            {
                if (objRecordSet != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(objRecordSet);
            }
            Logger.Debug(DebugString.Format(Messages.GetNextCodeEnd, udt));

            return (id);
        }

        public string GetCurrentUser()
        {
            return company.UserName;
        }

        public void ExecuteStatement(string sql)
        {
            var objRecordSet = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            Logger.Debug(DebugString.Format(Messages.StartExecuteStatement, sql));
            try
            {
                objRecordSet.DoQuery(sql);
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.ErrorExecuteStatement, sql, e.Message), e);
                throw e;
            }
            finally
            {
                if (objRecordSet != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(objRecordSet);
            }
            Logger.Debug(DebugString.Format(Messages.EndExecuteStatement, sql));
        }

        public T ExecuteSqlForObject<T>(string sql)
        {
            Type type = typeof(T);
            Recordset rs = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            try
            {
                rs.DoQuery(sql);
                int fieldsSize = rs.Fields.Count;
                if (!rs.EoF)
                {
                    if (!IsNotCoreType(type))
                    {
                        object obj = rs.Fields.Item(0).Value;
                        if (obj.GetType() != type)
                        {
                            String errMsg = String.Format(
                                Messages.ExecuteForObjectArgument,
                                obj.GetType(), type);
                            Logger.Error(errMsg);
                            throw new ArgumentException(errMsg);
                        }
                        Logger.Debug(DebugString.Format(Messages.ExecuteForObjectReturn, obj, sql));
                        return (T)obj;
                    }
                    else
                    {
                        var ret = PrepareObject<T>(rs);
                        Logger.Debug(DebugString.Format(Messages.ExecuteForObjectReturn, ret, sql));
                        return ret;
                    }
                }
                return default(T);
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.ExecuteForObjectError, e.Message));
                throw e;
            }
            finally
            {
                if (rs != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
            }
        }

        private bool IsNotCoreType(Type type)
        {
            return (type != typeof(object) && Type.GetTypeCode(type) == TypeCode.Object);
        }

        public List<T> ExecuteSqlForList<T>(string sql)
        {
            var retValue = new List<T>();
            Type type = typeof(T);
            Recordset rs = (Recordset)company.GetBusinessObject(BoObjectTypes.BoRecordset);
            Logger.Debug(DebugString.Format(Messages.ExecuteForListCommand, sql));

            try
            {
                rs.DoQuery(sql);
                int fieldsSize = rs.Fields.Count;
                while (!rs.EoF)
                {
                    T obj;
                    if (IsNotCoreType(type))
                        obj = PrepareObject<T>(rs);
                    else
                    {
                        if (rs.Fields.Item(0).Value.GetType() != type)
                        {
                            String errMsg = String.Format(
                                Messages.ExecuteForObjectArgument,
                                rs.Fields.Item(0).Value.GetType(), type);
                            Logger.Error(errMsg);
                            throw new Exception(errMsg);
                        }
                        obj = (T)rs.Fields.Item(0).Value;
                    }
                    Logger.Debug(DebugString.Format(Messages.ExecuteForListReturn, obj));
                    retValue.Add(obj);
                    rs.MoveNext();
                }
                return retValue;
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.ExecuteForObjectError, e.Message), e);
                throw e;
            }
            finally
            {
                if (rs != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
            }
        }

        private T PrepareObject<T>(Recordset rs)
        {
            Type type = typeof(T);
            T obj = (T)Activator.CreateInstance(type);
            for (int i = 0; i < rs.Fields.Count; i++)
            {
                string name = rs.Fields.Item(i).Name;
                try
                {
                    object value = rs.Fields.Item(i).Value;
                    var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (prop == null)
                    {
                        String errMsg = String.Format(Messages.PrepareObjectMissingParameter, type, name);
                        Logger.Error(errMsg);
                        throw new Exception(errMsg);
                    }
                    if (prop.PropertyType != value.GetType())
                    {
                        String errMsg = String.Format(
                            Messages.PrepareObjectInvalidParameter,
                            type, name, prop.PropertyType, value.GetType());
                        Logger.Error(errMsg);
                        throw new Exception(errMsg);
                    }
                    prop.SetValue(obj, value, null);
                }
                catch (Exception e)
                {
                    Logger.Error(String.Format(Messages.PrepareObjectError, name), e);
                    throw e;
                }
            }
            return obj;
        }

        public T GetBOMFromXML<T>(Stream stream)
        {
            try
            {
                var deserializer = new XmlSerializer(typeof(T));
                var bom = (T)deserializer.Deserialize(stream);
                return bom;
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.GetBomFromXMLError, e.Message), e);
                throw e;
            }
        }

        private object[] ProcessTuple(object tuple)
        {
            var type = tuple.GetType();
            object[] ret;

            if (type == typeof(Tuple<object>))
            {
                var ntuple = (Tuple<object>)tuple;
                ret = new object[] { ntuple.Item1 };
            }
            else if (type == typeof(Tuple<object, object>))
            {
                var ntuple = (Tuple<object, object>)tuple;
                ret = new object[] { ntuple.Item1, ntuple.Item2 };
            } 
            else 
            {
                throw new ArgumentException(String.Format(Messages.ProcessTupleError, type));
            }

            return ret;
        }

        public string GetXMLBom<V>(object[] keys, BoObjectTypes objType)
        {
            XDocument bom = null;
            Type type = typeof(V);
            V obj = default(V);
            company.XMLAsString = true;
            company.XmlExportType = BoXmlExportTypes.xet_ExportImportMode;

            foreach (var key in keys)
            {
                try
                {
                    obj = (V)company.GetBusinessObject(objType);
                    bool found = (bool)type.InvokeMember("GetByKey", BindingFlags.InvokeMethod | BindingFlags.Public, null, obj,
                        ProcessTuple(key));
                    if (found)
                    {
                        var xml = (string)type.InvokeMember("GetAsXML", BindingFlags.InvokeMethod | BindingFlags.Public, null, obj, null);
                        XDocument tempDoc = XDocument.Parse(xml);
                        if (bom == null)
                        {
                            bom = tempDoc;
                        }
                        else
                        {
                            XElement tempBO = tempDoc.Element("BOM").Element("BO");
                            if (tempBO != null)
                                bom.Element("BOM").Add(tempBO);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(String.Format(Messages.GetXMLBOMError, e.Message), e);
                    throw e;
                }
                finally
                {
                    if (obj != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                }
            }
            return bom.ToString();
        }

        public void UpdateOrSavePermissionIfNotExists(Attribute.PermissionAttribute permissionAttribute)
        {
            UserPermissionTree userPermissionTree = null;

            Logger.Debug(DebugString.Format(Messages.PermissionStart, permissionAttribute.PermissionID));
            try
            {
                userPermissionTree = (UserPermissionTree)company.GetBusinessObject(BoObjectTypes.oUserPermissionTree);
                if (userPermissionTree.GetByKey(permissionAttribute.PermissionID))
                {
                    UpdatePermission(userPermissionTree, permissionAttribute);
                }
                else
                {
                    AddPermission(userPermissionTree, permissionAttribute);
                }
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.UpdateOrSavePermissionError, e.Message), e);
                throw e;
            }
            finally
            {
                if (userPermissionTree != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userPermissionTree);
            }
            Logger.Debug(DebugString.Format(Messages.PermissionEnd, permissionAttribute.PermissionID));

        }

        private void AddPermission(UserPermissionTree userPermissionTree, Attribute.PermissionAttribute permissionAttribute)
        {
            userPermissionTree.PermissionID = permissionAttribute.PermissionID;
            userPermissionTree.Name = permissionAttribute.Name;
            userPermissionTree.ParentID = permissionAttribute.ParentID;
            userPermissionTree.UserPermissionForms.FormType = permissionAttribute.FormType;
            userPermissionTree.Options = permissionAttribute.Options;

            int ret = userPermissionTree.Add();
            if (ret != 0)
            {
                string err;
                company.GetLastError(out ret, out err);
                string exceptionErr = String.Format(Messages.PermissionError, permissionAttribute.PermissionID, err);
                Logger.Error(exceptionErr);
            }
            else
            {
                Logger.Info(String.Format(Messages.PermissionSuccess, permissionAttribute.PermissionID));
            }

        }

        private void UpdatePermission(UserPermissionTree userPermissionTree, Attribute.PermissionAttribute permissionAttribute)
        {
            var currPerm = XDocument.Parse(userPermissionTree.GetAsXML());
            userPermissionTree.PermissionID = permissionAttribute.PermissionID;
            userPermissionTree.Name = permissionAttribute.Name;
            userPermissionTree.ParentID = permissionAttribute.ParentID;
            userPermissionTree.UserPermissionForms.FormType = permissionAttribute.FormType;
            userPermissionTree.Options = permissionAttribute.Options;
            var attrPerm = XDocument.Parse(userPermissionTree.GetAsXML());

            if (!XDocument.DeepEquals(currPerm, attrPerm))
            {
                int ret = userPermissionTree.Update();
                if (ret != 0)
                {
                    string err;
                    company.GetLastError(out ret, out err);
                    string exceptionErr = String.Format(Messages.PermissionUpdateError, permissionAttribute.PermissionID, err);
                    Logger.Error(exceptionErr);
                }
                else
                {
                    Logger.Info(String.Format(Messages.PermissionUpdateSuccess, permissionAttribute.PermissionID));
                }
            }
        }


        public bool IsSuperUser()
        {
            string superUser = ExecuteSqlForObject<string>(String.Format(
                this.GetSQL("IsSuperUser.sql"), company.UserName));

            return superUser.Return(x => x == "Y", false);
        }

        public bool PermissionExists(Attribute.PermissionAttribute permissionAttribute)
        {
            UserPermissionTree userPermissionTree = null;

            Logger.Debug(DebugString.Format(Messages.PermissionStart, permissionAttribute.PermissionID));
            try
            {
                userPermissionTree = (UserPermissionTree)company.GetBusinessObject(BoObjectTypes.oUserPermissionTree);
                if (userPermissionTree.GetByKey(permissionAttribute.PermissionID))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(String.Format(Messages.UpdateOrSavePermissionError, e.Message), e);
                throw e;
            }
            finally
            {
                if (userPermissionTree != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userPermissionTree);
                Logger.Debug(DebugString.Format(Messages.PermissionEnd, permissionAttribute.PermissionID));
            }
        }

        public dynamic GetBusinessObject(BoObjectTypes objType)
        {
            return company.GetBusinessObject(objType);
        }

        public void UpdateBusinessObject(object b1Object)
        {
            CallBusinessObjectMethod(b1Object, "Update");
        }

        public void SaveBusinessObject(object b1Object)
        {
            CallBusinessObjectMethod(b1Object, "Save");
        }

        private void CallBusinessObjectMethod(object b1Object, string method)
        {
            Type type = b1Object.GetType();
            try
            {
                int ret = (int)type.InvokeMember(method, BindingFlags.InvokeMethod | BindingFlags.Public, null, b1Object, null);
                if (ret != 0)
                {
                    string err;
                    company.GetLastError(out ret, out err);
                    Release(b1Object);
                    throw new Exception(err);
                }
            }
            catch (Exception e)
            {
                Release(b1Object);
                throw e;
            }
        }

        public void Release(object b1Object)
        {
            if (b1Object != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(b1Object);
            }
        }
    }
}
