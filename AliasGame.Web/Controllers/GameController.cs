using System;
using System.Collections.Generic;
using AliasGame.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AliasGame.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class GameController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IUserService _userService;

        public GameController(ISessionService sessionService, IUserService userService)
        {
            _sessionService = sessionService;
            _userService = userService;
        }
        
        /*
         * {
         *     "status": true/false,
         *     "sessionId": "session id"
         * }
         * 
         */
        [HttpPost("create_session/{accessToken}")]
        public IActionResult CreateSession(string accessToken)
        {
            var user = _userService.GetUserInfo(accessToken);
            var opStatus = user != null;
            string createdSessionId = default;

            if (opStatus)
            {
                createdSessionId = _sessionService.CreateSession(user.Id);
                opStatus = createdSessionId != default;
            }

            return Ok(new
            {
                status = opStatus,
                sessionId = createdSessionId
            });
        }

        /*
         * {
         *     "status": true/false,
         *     "firstPlayerId": "1st player id",
         *     "secondPlayerId": "2nd player id",
         *     "thirdPlayerId": "3rd player id"
         *     "fourthPlayerId": "4th player id"
         * }
         * 
         */
        [HttpGet("view_session/{accessToken}/{sessionId")]
        public IActionResult ViewSession(string accessToken, string sessionId)
        {
            var user = _userService.GetUserInfo(accessToken);
            var opStatus = user != null;
            string firstPlayerId = default;
            string secondPlayerId = default;
            string thirdPlayerId = default;
            string fourthPlayerId = default;
            
            if (opStatus)
            {
                var dict = _sessionService.GetSessionInfo(sessionId);
                try
                {
                    firstPlayerId = dict[1];
                    secondPlayerId = dict[2];
                    thirdPlayerId = dict[3];
                    fourthPlayerId = dict[4];
                }
                catch (KeyNotFoundException e)
                {
                    opStatus = false;
                }
            }
            
            return Ok(new
            {
                status = opStatus,
                firstPlayerId = firstPlayerId,
                secondPlayerId = secondPlayerId,
                thirdPlayerId = thirdPlayerId,
                fourthPlayerId = fourthPlayerId
            });
        }

        
        /*
         * {
         *     "status": true/false
         * }
         * 
         */
        [HttpPost("join_session/{accessToken}/{sessionId}/{teamCode}")]
        public IActionResult JoinSession(string accessToken, string sessionId, int teamCode)
        {
            var user = _userService.GetUserInfo(accessToken);
            var opStatus = user != null && Enum.IsDefined(typeof(ISessionService.Team), teamCode);

            if (opStatus)
            {
                var team = (ISessionService.Team) teamCode;
                opStatus = _sessionService.JoinSession(user.Id, sessionId, team);
            }

            return Ok(new
            {
                status = opStatus
            });
        }

        /*
         * {
         *     "status": true/false
         * }
         * 
         */
        [HttpPost("leave_session/{accessToken}/{sessionId}")]
        public IActionResult LeaveSession(string accessToken, string sessionId)
        {
            var user = _userService.GetUserInfo(accessToken);
            var opStatus = user != null;

            if (opStatus)
            {
                opStatus = _sessionService.LeaveSession(user.Id.ToString(), sessionId);
            }

            return Ok(new
            {
                status = opStatus
            });
        }
    }
}