﻿namespace AzureBot.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class General
    {
        private static BotHelper botHelper;
        public static TestContext testContext { get; set; }

        internal static BotHelper BotHelper
        {
            get { return botHelper; }
        }

        // Will run once before all of the tests in the project. We start assuming the user is already logged in to Azure,
        // which should  be done separately via the AzureBot.ConsoleConversation or some other means. 
        [AssemblyInitialize]
        public static void SetUp(TestContext context)
        {
            testContext = context;
            string directLineToken = context.Properties["DirectLineToken"].ToString();
            string microsoftAppId = context.Properties["MicrosoftAppId"].ToString();
            string fromUser = context.Properties["FromUser"].ToString();
            string botId = context.Properties["BotId"].ToString();

            botHelper = new BotHelper(directLineToken, microsoftAppId, fromUser, botId);

            var subscription = context.GetSubscription();

            var testCase = new BotTestCase()
            {
                Action = $"select subscription {subscription}",
                ExpectedReply = $"Setting {subscription} as the current subscription. What would you like to do next?",
            };

            TestRunner.RunTestCase(testCase).Wait();

            try
            {
                TestRunner.EnsureAllVmsStopped().Wait();
            }
            catch
            {
                Console.WriteLine("CleanUp called from SetUp failed");
            }
        }

        // Will run after all the tests have finished
        [AssemblyCleanup]
        public static void CleanUp()
        {
            try
            {
                TestRunner.EnsureAllVmsStopped().Wait();
            }
            finally
            {
                if (botHelper != null)
                {
                    botHelper.Dispose();
                }
            }
        }
    }
}
