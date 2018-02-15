using AutoMapper;
using Company.Api.Rest.Data;
using Company.Common.Data;
using Company.Manager.Membership.Interface;
using Company.ServiceFabric.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Company.Api.Rest.Service
{
    [Route("api/[controller]")]
    public class UsersController
        : Controller
    {
        private readonly IMapper _Mapper;

        public UsersController(IMapper mapper)
        {
            _Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Post([FromBody]RegisterRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var request = _Mapper.Map<RegisterRequest>(requestDto);
                IMembershipManager proxy = AuditableProxy.ForMicroservice<IMembershipManager>();
                string result = await proxy.RegisterMemberAsync(request);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
            }
            return BadRequest(HttpStatusCode.BadRequest);
        }
    }
}
