namespace OmniCommon.Interfaces
{
    using System.Collections.Generic;
    using OmniCommon.Domain;

    public interface IClippingRepository
    {
        IList<Clipping> GetForLast24Hours();

        void Save(Clipping clip);
    }
}
