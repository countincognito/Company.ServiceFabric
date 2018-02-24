using AutoMapper;
using Company.Api.Rest.Data;
using Company.Api.Rest.Interface;
using Company.Common.Data;
using Company.Manager.Membership.Interface;
using Company.Utility.Audit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Company.Api.Rest.Service
{
    [Route("api/[controller]")]
    public class UsersController
        : Controller
    {
        private readonly IMapper _Mapper;
        private readonly IMembershipManager _MembershipManager;
        private readonly ILogger<IRestApi> _Logger;

        public UsersController(
            IMapper mapper,
            IMembershipManager membershipManager,
            ILogger<IRestApi> logger)
        {
            _Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _MembershipManager = membershipManager ?? throw new ArgumentNullException(nameof(membershipManager));
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Just in case we decide to go InProc.
            AuditContext.NewCurrentIfEmpty();

            Debug.Assert(AuditContext.Current != null);
            AuditContext.Current.AddExtraHeader("Jurisdiction", "UK");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Post([FromBody]RegisterRequestDto requestDto)
        {
            _Logger.LogInformation($"{nameof(Post)} Invoked");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var request = _Mapper.Map<RegisterRequest>(requestDto);
                string result = await _MembershipManager.RegisterMemberAsync(request);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    return Ok(result);
                }
            }
            catch (Exception e)
            {
                _Logger.LogError(e.ToString());
            }
            return BadRequest(HttpStatusCode.BadRequest);
        }
    }
}
