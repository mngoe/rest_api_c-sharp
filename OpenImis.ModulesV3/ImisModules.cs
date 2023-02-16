﻿using System;
using System.Linq;
using System.Reflection;
using OpenImis.ModulesV3.Helpers;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenImis.Security;
using OpenImis.ModulesV3.ClaimModule;
using OpenImis.ModulesV3.InsureeModule;
using OpenImis.ModulesV3.CoverageModule;
using OpenImis.ModulesV3.MasterDataModule;
using OpenImis.ModulesV3.MasterDataModule.Logic;
using OpenImis.ModulesV3.FeedbackModule;
using Microsoft.AspNetCore.Hosting;
using OpenImis.ModulesV3.PremiumModule;
using OpenImis.ModulesV3.SystemModule;
using OpenImis.ModulesV3.PolicyModule;
using OpenImis.ModulesV3.PolicyModule.Logic;
using OpenImis.ModulesV3.ReportModule;
using OpenImis.ModulesV3.ReportModule.Logic;

namespace OpenImis.ModulesV3
{
    public class ImisModules : IImisModules
    {
        private ILoginModule loginModule;

        private IInsureeModule insureeModule;
        private ClaimModule.ClaimModule claimModule;
        private ICoverageModule coverageModule;
        private IMasterDataModule masterDataModule;
        private IFeedbackModule feedbackModule;
        private IPremiumModule premiumModule;
        private ISystemModule systemModule;
        private IPolicyModule policyModule;
        private IReportModule reportModule;

        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public ImisModules(IConfiguration configuration, IHostingEnvironment hostingEnvironment, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger("LoggerCategory");
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates and returns the login module version 2.
        /// </summary>
        /// <returns>
        /// The Login module V2.
        /// </returns>
        //public ILoginModule GetLoginModule()
        //{
        //    if (loginModule == null)
        //    {
        //        loginModule = new Security.LoginModule.LoginModule(_configuration);

        //        Type loginLogicType = CreateTypeFromConfiguration( "LoginModule", "LoginLogic", "OpenImis.Security.Logic.LoginLogic");
        //        loginModule.SetLoginLogic((Security.Logic.ILoginLogic)ActivatorUtilities.CreateInstance(_serviceProvider, loginLogicType));
        //    }
        //    return loginModule;
        //}

        /// <summary>
        /// Creates and returns the claim module version 2.
        /// </summary>
        /// <returns>
        /// The Claim module V2.
        /// </returns>
        public ClaimModule.ClaimModule GetClaimModule()
        {
            if (claimModule == null)
            {
                claimModule = new ClaimModule.ClaimModule(_configuration, _hostingEnvironment, _loggerFactory);

                Type claimLogicType = CreateTypeFromConfiguration("ClaimModule", "ClaimLogic", "OpenImis.ModulesV3.ClaimModule.Logic.ClaimLogic");
                claimModule.SetClaimLogic((ClaimModule.Logic.ClaimLogic)ActivatorUtilities.CreateInstance(_serviceProvider, claimLogicType));
            }
            return claimModule;
        }

        /// <summary>
        /// Creates and returns the insuree module version 2.
        /// </summary>
        /// <returns>
        /// The Insuree module V2.
        /// </returns>
        public IInsureeModule GetInsureeModule()
        {
            if (insureeModule == null)
            {
                insureeModule = new InsureeModule.InsureeModule(_configuration, _hostingEnvironment, _loggerFactory);

                Type familyLogicType = CreateTypeFromConfiguration("InsureeModule", "FamilyLogic", "OpenImis.ModulesV3.InsureeModule.Logic.FamilyLogic");
                insureeModule.SetFamilyLogic((InsureeModule.Logic.IFamilyLogic)ActivatorUtilities.CreateInstance(_serviceProvider, familyLogicType));

                Type contributionLogicType = CreateTypeFromConfiguration("InsureeModule", "ContributionLogic", "OpenImis.ModulesV3.InsureeModule.Logic.ContributionLogic");
                insureeModule.SetContributionLogic((InsureeModule.Logic.IContributionLogic)ActivatorUtilities.CreateInstance(_serviceProvider, contributionLogicType));

                Type insureeLogicType = CreateTypeFromConfiguration("InsureeModule", "InsureeLogic", "OpenImis.ModulesV3.InsureeModule.Logic.InsureeLogic");
                insureeModule.SetInsureeLogic((InsureeModule.Logic.IInsureeLogic)ActivatorUtilities.CreateInstance(_serviceProvider, insureeLogicType));
            }
            return insureeModule;
        }

        /// <summary>
        /// Creates and returns the payment module version 2.
        /// </summary>
        /// <returns>
        /// The Payment module V2.
        /// </returns>
        public ICoverageModule GetCoverageModule()
        {
            if (coverageModule == null)
            {
                coverageModule = new CoverageModule.CoverageModule(_configuration);

                Type coverageLogicType = CreateTypeFromConfiguration("CoverageModule", "CoverageLogic", "OpenImis.ModulesV3.CoverageModule.Logic.CoverageLogic");
                coverageModule.SetCoverageLogic((CoverageModule.Logic.ICoverageLogic)ActivatorUtilities.CreateInstance(_serviceProvider, coverageLogicType));
            }
            return coverageModule;
        }

        /// <summary>
        /// Creates and returns the feedback module version 2.
        /// </summary>
        /// <returns>
        /// The Feedback module V2.
        /// </returns>
        public IFeedbackModule GetFeedbackModule()
        {
            if (feedbackModule == null)
            {
                feedbackModule = new FeedbackModule.FeedbackModule(_configuration, _hostingEnvironment);

                Type feedbackLogicType = CreateTypeFromConfiguration("FeedbackModule", "FeedbackLogic", "OpenImis.ModulesV3.FeedbackModule.Logic.FeedbackLogic");
                feedbackModule.SetFeedbackLogic((FeedbackModule.Logic.IFeedbackLogic)ActivatorUtilities.CreateInstance(_serviceProvider, feedbackLogicType));
            }
            return feedbackModule;
        }

        /// <summary>
        /// Creates and returns the premium module version 2.
        /// </summary>
        /// <returns>
        /// The Premium module V2.
        /// </returns>
        public IPremiumModule GetPremiumModule()
        {
            if (premiumModule == null)
            {
                premiumModule = new PremiumModule.PremiumModule(_configuration);

                Type premiumLogicType = CreateTypeFromConfiguration("PremiumModule", "PremiumLogic", "OpenImis.ModulesV3.PremiumModule.Logic.PremiumLogic");
                premiumModule.SetPremiumLogic((PremiumModule.Logic.IPremiumLogic)ActivatorUtilities.CreateInstance(_serviceProvider, premiumLogicType));
            }
            return premiumModule;
        }

        /// <summary>
        /// Creates and returns the system module version 2.
        /// </summary>
        /// <returns>
        /// The System module V2.
        /// </returns>
        public ISystemModule GetSystemModule()
        {
            if (systemModule == null)
            {
                systemModule = new SystemModule.SystemModule(_configuration);

                Type systemLogicType = CreateTypeFromConfiguration("SystemModule", "SystemLogic", "OpenImis.ModulesV3.SystemModule.Logic.SystemLogic");
                systemModule.SetSystemLogic((SystemModule.Logic.ISystemLogic)ActivatorUtilities.CreateInstance(_serviceProvider, systemLogicType));
            }
            return systemModule;
        }

        /// <summary>
        /// Creates and returns the master data module version 2.
        /// </summary>
        /// <returns>
        /// The MasterData module V2.
        /// </returns>
        public IMasterDataModule GetMasterDataModule()
        {
            if (masterDataModule == null)
            {
                masterDataModule = new MasterDataModule.MasterDataModule(_configuration);

                Type masterDataLogicType = CreateTypeFromConfiguration("MasterDataModule", "MasterDataLogic", "OpenImis.ModulesV3.MasterDataModule.Logic.MasterDataLogic");
                masterDataModule.SetMasterDataLogic((IMasterDataLogic)ActivatorUtilities.CreateInstance(_serviceProvider, masterDataLogicType));
            }
            return masterDataModule;
        }

        /// <summary>
        /// Creates and returns the policy module version 2.
        /// </summary>
        /// <returns>
        /// The Policy module V2.
        /// </returns>
        public IPolicyModule GetPolicyModule()
        {
            if (policyModule == null)
            {
                policyModule = new PolicyModule.PolicyModule(_configuration, _hostingEnvironment, _loggerFactory);

                Type policyLogicType = CreateTypeFromConfiguration("PolicyModule", "PolicyRenewalLogic", "OpenImis.ModulesV3.PolicyModule.Logic.PolicyRenewalLogic");
                policyModule.SetPolicyLogic((IPolicyRenewalLogic)ActivatorUtilities.CreateInstance(_serviceProvider, policyLogicType));
            }
            return policyModule;
        }

        /// Creates and returns the report module version 2.
        /// </summary>
        /// <returns>
        /// The Report module V2.
        /// </returns>
        public IReportModule GetReportModule()
        {
            if (reportModule == null)
            {
                reportModule = new ReportModule.ReportModule(_configuration);

                Type reportLogicType = CreateTypeFromConfiguration("ReportModule", "ReportLogic", "OpenImis.ModulesV3.ReportModule.Logic.ReportLogic");
                reportModule.SetReportLogic((IReportLogic)ActivatorUtilities.CreateInstance(_serviceProvider, reportLogicType));
            }
            return reportModule;
        }

        /// <summary>
		/// Creates and returns the type based on the string from the configuration 
		/// </summary>
		/// <param name="moduleName">The module name</param>
		/// <param name="sectionName">The section name</param>
		/// <param name="defaultValue">The default section value</param>
		/// <returns>Type represented by the section</returns>
		private Type CreateTypeFromConfiguration(string moduleName, string sectionName, string defaultValue)
        {
            Type type;

            Assembly assembly = Assembly.GetCallingAssembly();
            string part = GetSectionName(moduleName, sectionName, "3");
            type = assembly.GetType(part);
            if (type == null)
            {
                _logger.LogError(moduleName + " " + sectionName + " error: the type " + part + " was not found. Using default " + defaultValue + " configuration.");
                type = assembly.GetType(defaultValue);
            }
            else
            {
                _logger.LogInformation(moduleName + " load OK: " + part);
            }

            return type;
        }

        public string GetSectionName(string moduleName, string sectionName, string apiVersion)
        {
            string part = "";

            var listImisModules = _configuration.GetSection("ImisModules").Get<List<ConfigImisModules>>();
           
            var module = listImisModules.Where(m => m.Version == apiVersion).Select(x => GetPropValue(x, moduleName)).FirstOrDefault();

            if (GetPropValue(module, sectionName) != null)
            {
                part = GetPropValue(module, sectionName).ToString();
            }
            return part;
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
