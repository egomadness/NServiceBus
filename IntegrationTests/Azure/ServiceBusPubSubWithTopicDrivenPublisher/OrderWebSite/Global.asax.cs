﻿using System;
using System.Collections.Generic;
using System.Web;
using log4net;
using MyMessages;
using NServiceBus;

namespace OrderWebSite
{
    public class Global : HttpApplication
    {
		public static IBus Bus;
		public static IList<MyMessages.Order> Orders;

		private static readonly Lazy<IBus> StartBus = new Lazy<IBus>(ConfigureNServiceBus);
		
		private static IBus ConfigureNServiceBus()
		{
		    Configure.Transactions.Enable();
            Configure.Serialization.Json();

            var bus = Configure.With()
                .DefaultBuilder()
                .AzureConfigurationSource()
                .AzureServiceBusMessageQueue()
                    .QueuePerInstance()
                .DisableTimeoutManager()
                .UnicastBus()
                .CreateBus()
				.Start();

			bus.Send(new LoadOrdersMessage());

			return bus;
		}

    	protected void Application_Start(object sender, EventArgs e)
        {
			Orders = new List<MyMessages.Order>();
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			Bus = StartBus.Value;
		}

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
           
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}