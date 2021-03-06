﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nimbus.IntegrationTests.Extensions;
using Nimbus.IntegrationTests.Tests.PoisonMessageTests.MessageContracts;
using Nimbus.Tests.Common;
using NUnit.Framework;
using Shouldly;

namespace Nimbus.IntegrationTests.Tests.PoisonMessageTests
{
    [TestFixture]
    public class WhenACommandFailsToBeHandledMoreThanNTimes : TestForBus
    {
        private GoBangCommand _goBangCommand;
        private string _someContent;
        private GoBangCommand[] _deadLetterMessages;

        private const int _maxDeliveryAttempts = 3;

        protected override async Task When()
        {
            _someContent = Guid.NewGuid().ToString();
            _goBangCommand = new GoBangCommand(_someContent);

            await Bus.Send(_goBangCommand);

            await TimeSpan.FromSeconds(10).WaitUntil(() => MethodCallCounter.AllReceivedCalls.Count() >= _maxDeliveryAttempts);

            _deadLetterMessages = (await FetchAllDeadLetterMessages(Bus)).ToArray();
        }

        [Test]
        public async Task ThereShouldBeExactlyOneMessageOnTheDeadLetterQueue()
        {
            _deadLetterMessages.Count().ShouldBe(1);
            _deadLetterMessages.Single().SomeContent.ShouldBe(_someContent);
        }

        private static async Task<List<GoBangCommand>> FetchAllDeadLetterMessages(IBus bus)
        {
            var deadLetterMessages = new List<GoBangCommand>();
            while (true)
            {
                var message = await bus.DeadLetterQueues.CommandQueue.Pop<GoBangCommand>();
                if (message == null) break;
                deadLetterMessages.Add(message);
            }
            return deadLetterMessages;
        }
    }
}