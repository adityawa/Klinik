using AutoMapper;
using Klinik.Common;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Account;
using Klinik.Entities.Administration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Klinik.Features
{
    /// <summary>
    /// Abstract class of base features
    /// </summary>
    public abstract class BaseFeatures
    {
        public static IUnitOfWork _unitOfWork { get; set; }
        public KlinikDBEntities _context;
        public IList<string> errorFields = new List<string>();

        /// <summary>
        /// Contructor
        /// </summary>
        public BaseFeatures()
        {
        }

        /// <summary>
        /// Constructor with parameter
        /// </summary>
        /// <param name="unitOfWork"></param>
        public BaseFeatures(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Validate the privilege based on its name
        /// </summary>
        /// <param name="privilege_name"></param>
        /// <param name="PrivilegeIds"></param>
        /// <returns></returns>
        public bool IsHaveAuthorization(string privilege_name, List<long> PrivilegeIds)
        {
            bool IsAuthorized = false;
            var _getPrivilegeName = _unitOfWork.PrivilegeRepository.Get(x => PrivilegeIds.Contains(x.ID));

            foreach (var item in _getPrivilegeName)
            {
                if (privilege_name == item.Privilege_Name)
                    IsAuthorized = true;
            }

            return IsAuthorized;
        }

        /// <summary>
        /// Log any executed command by user
        /// </summary>
        /// <param name="module"></param>
        /// <param name="status"></param>
        /// <param name="command"></param>
        /// <param name="account"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        public static void CommandLog(ClinicEnums.Module module, ClinicEnums.Status status, string command, AccountModel account, object newValue = null, object oldValue = null)
        {
            try
            {
                var log = new LogModel
                {
                    Start = DateTime.Now,
                    Module = module.ToString(),
                    Status = status.ToString(),
                    Command = command,
                    UserName = account.UserName,
                    Organization = account.Organization,
                    OldValue = oldValue is null ? null : JsonConvert.SerializeObject(oldValue),
                    NewValue = newValue is null ? null : JsonConvert.SerializeObject(newValue)
                };

                var _entity = Mapper.Map<LogModel, Log>(log);

                _unitOfWork.LogRepository.Insert(_entity);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(ex.GetAllMessages(), EventLogEntryType.Error, 101, 1);
                }
            }
        }
    }
}