using AutoMapper;
using Company.Api.Rest.Data;
using Company.Common.Data;
using Company.Manager.Membership.Interface;
using Company.Utility;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
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
        private readonly ILogger _Logger;

        public UsersController(
            IMapper mapper,
            IMembershipManager membershipManager,
            ILogger logger)
        {
            _Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _MembershipManager = membershipManager ?? throw new ArgumentNullException(nameof(membershipManager));
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Just in case we decide to go InProc.
            TrackingContext tc = TrackingContext.NewCurrentIfEmpty(new Dictionary<string, string>() { { "Jurisdiction", "UK" } });

            Debug.Assert(tc != null);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Post([FromBody]RegisterRequestDto requestDto)
        {
            _Logger.Information($"{nameof(Post)} Invoked");
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
                _Logger.Error(e.ToString());
            }
            return BadRequest(HttpStatusCode.BadRequest);
        }
    }
}
