﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceWizard
{
    public static class DBObjectFactory
    {

        public static IDBObject New(Type dbObjType)
        {
            if(!(typeof(IDBObject).IsAssignableFrom(dbObjType)))
            {
                throw new Exceptions.NotADBObjectException();
            }
            // Creating through parameterless constructor.
            return (IDBObject)Activator.CreateInstance(dbObjType);
        }

        /// <summary>
        /// Gets the type of object the given script describes.
        /// </summary>
        /// <param name="script">Script to analyse.</param>
        /// <returns></returns>
        public static Type GetDBTypeFromScript(string script)
        {
            string scriptCaseInsensitive = script.ToLower();

            if(scriptCaseInsensitive.Contains("create proc") || scriptCaseInsensitive.Contains ("alter proc"))
            {
                return typeof(DBProcedure);
            }

            if(scriptCaseInsensitive.Contains("create view") || scriptCaseInsensitive.Contains ("alter view"))
            {
                return typeof(DBView);
            }

            if(scriptCaseInsensitive.Contains("create table") || scriptCaseInsensitive.Contains("alter table"))
            {
                return typeof(DBTable);
            }

            if (scriptCaseInsensitive.Contains("create function") || scriptCaseInsensitive.Contains("alter table"))
            {
                return typeof(DBFunction);
            }

            return null;
        }
    }
}
