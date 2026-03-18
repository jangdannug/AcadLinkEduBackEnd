using AcadLinkEduBackEnd.Application.Services;
using AcadLinkEduBackEnd.Domain.Entities;
using AcadLinkEduBackEnd.API.Models;
using Microsoft.AspNetCore.Mvc;
using Supabase.Postgrest.Exceptions;

namespace AcadLinkEduBackEnd.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                var dtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.Name,
                    Role = u.Role,
                    IsVerified = u.IsVerified
                }).ToList();

                return Ok(dtos);
            }
            catch (PostgrestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto input)
        {
            try
            {
                var createdUser = await _userService.RegisterAsync(input.Email, input.Name, input.Role);
                var dto = new UserDto
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    Name = createdUser.Name,
                    Role = createdUser.Role,
                    IsVerified = createdUser.IsVerified
                };
                return CreatedAtAction(nameof(GetUsers), new { id = dto.Id }, dto);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Email already registered"))
            {
                return Conflict(new { message = ex.Message });
            }
            catch (PostgrestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto input)
        {
            try
            {
                var user = await _userService.LoginAsync(input.Email);
                var dto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = user.Role,
                    IsVerified = user.IsVerified
                };
                return Ok(dto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (PostgrestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}/verify")]
        public async Task<IActionResult> VerifyUser(int id)
        {
            try
            {
                var user = await _userService.VerifyUserAsync(id);
                var dto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = user.Role,
                    IsVerified = user.IsVerified
                };
                return Ok(dto);
            }
            catch (PostgrestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}/revoke")]
        public async Task<IActionResult> RovokeUser(int id)
        {
            try
            {
                var user = await _userService.RevokeUserAsync(id);
                var dto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = user.Role,
                    IsVerified = user.IsVerified
                };
                return Ok(dto);
            }
            catch (PostgrestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("batch-verify")]
        public async Task<IActionResult> BatchVerifyUsers([FromBody] int[] ids)
        {
            try
            {
                var result = await _userService.BatchVerifyUsersAsync(ids);
                return Ok(new { success = result });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (PostgrestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(id);
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (PostgrestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
