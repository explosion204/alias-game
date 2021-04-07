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
            var user = _userService.GetUserFromToken(accessToken);
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
         *     "nickname_1": "1st player id",
         *     "nickname_2": "2nd player id",
         *     "nickname_3": "3rd player id"
         *     "nickname_4": "4th player id"
         * }
         * 
         */
        [HttpGet("view_session/{accessToken}/{sessionId}")]
        public IActionResult ViewSession(string accessToken, string sessionId)
        {
            var user = _userService.GetUserFromToken(accessToken);
            var opStatus = user != null;
            string firstPlayerNickname = default;
            string secondPlayerNickname = default;
            string thirdPlayerNickname = default;
            string fourthPlayerNickname = default;
            
            if (opStatus)
            {
                var dict = _sessionService.GetSessionInfo(sessionId);
                try
                {
                    var firstPlayerId = dict[1];
                    var secondPlayerId = dict[2];
                    var thirdPlayerId = dict[3];
                    var fourthPlayerId = dict[4];
                    
                    firstPlayerNickname = _userService.GetUserById(firstPlayerId)?.Nickname;
                    secondPlayerNickname = _userService.GetUserById(secondPlayerId)?.Nickname;
                    thirdPlayerNickname = _userService.GetUserById(thirdPlayerId)?.Nickname;
                    fourthPlayerNickname = _userService.GetUserById(fourthPlayerId)?.Nickname;
                }
                catch (KeyNotFoundException e)
                {
                    opStatus = false;
                }
            }
            
            return Ok(new
            {
                status = opStatus,
                nickname_1 = firstPlayerNickname,
                nickname_2 = secondPlayerNickname,
                nickname_3 = thirdPlayerNickname,
                nickname_4 = fourthPlayerNickname
            });
        }

        
        /*
         * {
         *     "status": true/false,
         *     "position": 2, 3 or 4
         * }
         * 
         */
        [HttpPost("join_session/{accessToken}/{sessionId}/{teamCode}")]
        public IActionResult JoinSession(string accessToken, string sessionId, int teamCode)
        {
            var user = _userService.GetUserFromToken(accessToken);
            var opStatus = user != null && Enum.IsDefined(typeof(ISessionService.Team), teamCode);
            var position = 0;

            if (opStatus)
            {
                var team = (ISessionService.Team) teamCode;
                
                opStatus = _sessionService.JoinSession(user.Id, sessionId, team, out position);
            }

            return Ok(new
            {
                status = opStatus,
                position = position
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
            var user = _userService.GetUserFromToken(accessToken);
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