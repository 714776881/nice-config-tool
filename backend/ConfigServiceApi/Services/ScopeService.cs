using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ConfigServiceApi.Models;
using ConfigServiceApi.Repository;

namespace ConfigServiceApi.Services
{

    public enum RoleTypeEnum
    {
        SuperManager = 9011,
        HospitalManager = 9010,
        AreaManager = 9012,
        Other = 9999,
    }

    public class ScopeService
    {
        private ScopeRepository scopeRepository;
        private UserRepository userRepository;

        public ScopeService() { 
            scopeRepository = new ScopeRepository();
            userRepository = new UserRepository();
        }

        public List<ScopeEntity> GetOptions(string userId, string regionId, string hospitalId, string departmentId)
        {
            var scopes = scopeRepository.GetAll();
            var userEntity = userRepository.GetUserById(userId);
            if(userEntity == null)
            {
                throw new Exception("未发现该用户ID!userID"+userId);
            }

            var result = new List<ScopeEntity>();
            var roles = userEntity.UserRole.Split(';').ToList();
            if(roles.Contains(EnmuNumToString(RoleTypeEnum.SuperManager)))
            {
                result = scopes;
            }
            else if (roles.Contains(EnmuNumToString(RoleTypeEnum.AreaManager)))
            {
                result = scopes.Where((item) => item.RegionCode.Equals(regionId)).ToList();
            }
            else if (roles.Contains(EnmuNumToString(RoleTypeEnum.HospitalManager)))
            {
                result = scopes.Where((item) => item.RegionCode.Equals(regionId) && item.HospitalCode.Equals(hospitalId)).ToList();
            }
            else
            {
                result = scopes.Where((item) => item.RegionCode.Equals(regionId) && item.HospitalCode.Equals(hospitalId) && item.DepartmentCode.Equals(hospitalId)).ToList();
            }
            return result;
        }

        private string EnmuNumToString(object obj)
        {
            return Convert.ToInt16((obj)).ToString();
        }
    }

}
