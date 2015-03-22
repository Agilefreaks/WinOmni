﻿﻿using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using Refit;
using SMS.Models;

/* ******** Hey You! *********
 *
 * This is a generated file, and gets rewritten every time you build the
 * project. If you want to edit it, you need to edit the mustache template
 * in the Refit package */

namespace RefitInternalGenerated
{
    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate)]
    sealed class PreserveAttribute : Attribute
    {
#pragma warning disable 0649
        //
        // Fields
        //
        public bool AllMembers;

        public bool Conditional;
#pragma warning restore 0649
    }
}

namespace SMS.Resources.v1
{
    using RefitInternalGenerated;

    [Preserve]
    public partial class AutoGeneratedISMSMessagesApi : ISMSMessagesApi
    {
        public HttpClient Client { get; protected set; }
        readonly Dictionary<string, Func<HttpClient, object[], object>> methodImpls;

        public AutoGeneratedISMSMessagesApi(HttpClient client, IRequestBuilder requestBuilder)
        {
            methodImpls = requestBuilder.InterfaceHttpMethods.ToDictionary(k => k, v => requestBuilder.BuildRestResultFuncForMethod(v));
            Client = client;
        }

        public virtual IObservable<SmsMessageDto> Get(string id,string token)
        {
            var arguments = new object[] { id,token };
            return (IObservable<SmsMessageDto>) methodImpls["Get"](Client, arguments);
        }

        public virtual IObservable<SmsMessageDto> Create(object payload,string token)
        {
            var arguments = new object[] { payload,token };
            return (IObservable<SmsMessageDto>) methodImpls["Create"](Client, arguments);
        }

    }
}

