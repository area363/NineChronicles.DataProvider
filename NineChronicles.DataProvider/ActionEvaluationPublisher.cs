namespace NineChronicles.DataProvider
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Lib9c.Renderer;
    using Microsoft.Extensions.Hosting;
    using Nekoyume.Action;
    using NineChronicles.Headless;
    using Serilog;
    using NineChroniclesActionType = Libplanet.Action.PolymorphicAction<Nekoyume.Action.ActionBase>;

    [SuppressMessage("ReSharper", "SA1309", Justification = "Use underscore for private field")]
    public class ActionEvaluationPublisher : BackgroundService
    {
        private readonly BlockRenderer _blockRenderer;
        private readonly ActionRenderer _actionRenderer;
        private readonly ExceptionRenderer _exceptionRenderer;
        private readonly NodeStatusRenderer _nodeStatusRenderer;

        public ActionEvaluationPublisher(
            BlockRenderer blockRenderer,
            ActionRenderer actionRenderer,
            ExceptionRenderer exceptionRenderer,
            NodeStatusRenderer nodeStatusRenderer)
        {
            this._blockRenderer = blockRenderer;
            this._actionRenderer = actionRenderer;
            this._exceptionRenderer = exceptionRenderer;
            this._nodeStatusRenderer = nodeStatusRenderer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._actionRenderer.EveryRender<ActionBase>()
                .Subscribe(
                 ev =>
                {
                    try
                    {
                        Log.Debug("***********ACTION: {0}", ev.Action.PlainValue.ToString());
                    }
                    catch (SerializationException se)
                    {
                        // add logger as property
                        Log.Error(se, "Skip broadcasting render since the given action isn't serializable.");
                    }
                    catch (Exception e)
                    {
                        // add logger as property
                        Log.Error(e, "Skip broadcasting render due to the unexpected exception");
                    }
                },
                 stoppingToken);
            return Task.CompletedTask;
        }
    }
}
