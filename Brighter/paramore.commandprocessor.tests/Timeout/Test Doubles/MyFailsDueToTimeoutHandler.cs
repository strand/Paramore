﻿using System.Threading;
using System.Threading.Tasks;
using paramore.commandprocessor.tests.CommandProcessors.TestDoubles;
using paramore.commandprocessor.timeoutpolicy.Attributes;
using paramore.commandprocessor.timeoutpolicy.Handlers;

namespace paramore.commandprocessor.tests.Timeout.TestDoubles
{
    internal class MyFailsDueToTimeoutHandler: RequestHandler<MyCommand>
    {
        public static bool WasCancelled { get; set; }

        [TimeoutPolicy(milliseconds: 1000, step: 1)]
        public override MyCommand Handle(MyCommand command)
        {
            var ct = (CancellationToken)Context.Bag[TimeoutPolicyHandler<MyCommand>.CONTEXT_BAG_TIMEOUT_CANCELLATION_TOKEN];
            if (ct.IsCancellationRequested)
            {
                //already died
                WasCancelled = true;
                return base.Handle(command);
            }
            else
            {
                Task.Delay(2000, ct).ContinueWith((antecedent) =>
                {
                    if (ct.IsCancellationRequested)
                        WasCancelled = true;
                }).Wait();

                return base.Handle(command);
            }
        }

    }
}