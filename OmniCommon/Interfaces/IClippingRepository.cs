namespace OmniCommon.Interfaces
{
    using System.Collections.Generic;
    using OmniCommon.Domain;

    public interface IClippingRepository
    {
        IList<Clipping> GetAll();

        void Save(Clipping clip);
    }
}
