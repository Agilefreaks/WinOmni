namespace OmniUI.Presenters
{
    using System;
    using Caliburn.Micro;
    using OmniUI.Entities;

    public interface IPresenter
    {
        IEntity BackingModel { get; set; }

        string Id { get; set; }

        bool IsDeleted { get; set; }

        DateTime Time { get; set; }

        string UniqueId { get; set; }

        bool WasViewed { get; set; }
    }

    public interface IPresenter<T> : IPresenter
        where T : Entity
    {
        new T BackingModel { get; set; }
    }

    public class Presenter : PropertyChangedBase, IPresenter
    {
        public Presenter(IEntity backingModel)
        {
            BackingModel = backingModel;
        }

        #region IPresenter Members

        public IEntity BackingModel { get; set; }

        public string Id
        {
            get
            {
                return BackingModel.Id;
            }

            set
            {
                BackingModel.Id = value;
            }
        }

        public bool IsDeleted
        {
            get
            {
                return BackingModel.IsDeleted;
            }
            set
            {
                BackingModel.IsDeleted = value;
                NotifyOfPropertyChange();
            }
        }

        public DateTime Time
        {
            get
            {
                return BackingModel.Time;
            }
            set
            {
                BackingModel.Time = value;
            }
        }

        public string UniqueId
        {
            get
            {
                return BackingModel.UniqueId;
            }
            set
            {
                BackingModel.UniqueId = value;
            }
        }

        public bool WasViewed
        {
            get
            {
                return BackingModel.WasViewed;
            }
            set
            {
                BackingModel.WasViewed = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion
    }

    public class Presenter<T> : Presenter, IPresenter<T>
        where T : Entity
    {
        public Presenter(T backingModel)
            : base(backingModel)
        {
            BackingModel = backingModel;
        }

        #region IPresenter<T> Members

        public new T BackingModel { get; set; }

        #endregion
    }
}