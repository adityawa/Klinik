﻿using AutoMapper;
using Klinik.Data;
using Klinik.Data.DataRepository;
using Klinik.Entities.Administration;
using System;
using System.Collections.Generic;

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

        public static void CommandLogging(LogModel log)
        {
            try
            {
                var _entity = Mapper.Map<LogModel, Log>(log);
                _unitOfWork.LogRepository.Insert(_entity);
                _unitOfWork.Save();
            }
            catch(Exception ex)
            {

            }
        }
    }
}