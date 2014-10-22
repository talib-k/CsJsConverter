﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CsJsConversion.BaseClassMembers
{
    public class CSharp
    {
        public CSharp()
        {
            InitMvc();
        }

        public EnumInfo Enum { get; private set; }

        public MvcInfo Mvc { get; private set; }

        public virtual EnumInfo GetEnumInfo<T>()
        {
            return new EnumInfo();
        }

        public virtual ClassInfo<T> GetClassInfo<T>()
        {
            return new ClassInfo<T>();
        }

        protected virtual void InitMvc()
        {
            Mvc = new MvcInfo();
        }
    }
}
