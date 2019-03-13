using Klinik.Data;
using Klinik.Entities.MasterData;
using Klinik.Features;
using System.Collections.Generic;
using System.Web.Http;

namespace Klinik.WebApi.Controllers
{
    /// <summary>
    /// Employee controller
    /// </summary>
    public class EmployeeController : ApiController
    {
        private IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        public EmployeeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get employee list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Get employee by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="model"></param>
        public void Post([FromBody]EmployeeModel model)
        {
            var request = new EmployeeRequest
            {
                Data = model
            };

            EmployeeResponse _response = new EmployeeValidator(_unitOfWork).Validate(request, true);
        }

        /// <summary>
        /// Delete certain employee
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
        }
    }
}
