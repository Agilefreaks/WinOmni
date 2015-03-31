namespace Omnipaste.Framework.Services.ExceptionReporters
{
    using System;
    using System.Collections.Generic;
    using OmniCommon.Helpers;

    public class CompositeExceptionReporter : IExceptionReporter
    {
        #region Fields

        private readonly List<IExceptionReporter> _reporters;

        #endregion

        #region Constructors and Destructors

        public CompositeExceptionReporter()
        {
            _reporters = new List<IExceptionReporter>();
        }

        public CompositeExceptionReporter(params IExceptionReporter[] reporters)
            : this()
        {
            _reporters.AddRange(reporters);
        }

        #endregion

        #region Public Methods and Operators

        public void Report(Exception exception)
        {
            _reporters.ForEach(reporter => reporter.Report(exception));
        }

        #endregion
    }
}