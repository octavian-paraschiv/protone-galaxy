using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace OPMedia.Core
{
    [DataContract]
    public enum ChangeType
    {
        [EnumMember]
        /// <summary>
        /// No change
        /// </summary>
        None = 0,

        [EnumMember]
        /// <summary>
        /// Saved (i.e. added or modified - both handled the same)
        /// </summary>
        Saved,

        [EnumMember]
        /// <summary>
        /// Deleted
        /// </summary>
        Deleted,
    }

    [ServiceContract(CallbackContract = typeof(IPersistenceNotification))]
    public interface IPersistenceService
    {
        [OperationContract(IsOneWay = true)]
        void Subscribe(string appId);

        [OperationContract(IsOneWay = true)]
        void Unsubscribe(string appId);

        [OperationContract(IsOneWay = false)]
        string ReadObject(string persistenceId, string persistenceContext);

        [OperationContract(IsOneWay = true)]
        void SaveObject(string persistenceId, string persistenceContext, string objectContent);

        [OperationContract(IsOneWay = true)]
        void DeleteObject(string persistenceId, string persistenceContext);
    }

    public interface IPersistenceNotification
    {
        [OperationContract(IsOneWay = true)]
        void Notify(ChangeType changeType, string persistenceId, string persistenceContext, string objectContent);
    }
}
