﻿using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MVCApp.Startup))]
namespace MVCApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}