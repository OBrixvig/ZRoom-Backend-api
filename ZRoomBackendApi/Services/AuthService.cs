using ZRoomLoginLibrary.Models;
using ZRoomLoginLibrary.Repositories;

namespace ZRoomBackendApi.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IUserRepository userRepository, JwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public UserResponseDTO Login(LoginDTO loginDTO)
        {
            var user = _userRepository.Authenticate(loginDTO);

            if (user == null) 
            {
                return null;
            }

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new UserResponseDTO { Token = token };
        }
    }
}
