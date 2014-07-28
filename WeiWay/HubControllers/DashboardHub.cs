using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiWay.HubControllers
{
    [HubName("dashboard")]
    public class DashboardHub: Hub
    {
    }
}