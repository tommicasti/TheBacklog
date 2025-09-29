export const BASE_API_URL = 'http://localhost:5141/api/';

// USER API Endpoints
export const LOGIN_API_URL = `${BASE_API_URL}auth/login`;
export const REGISTER_API_URL = `${BASE_API_URL}auth/register`;

// GAMES API Endpoints
export const POPULAR_GAMES_API_URL = `${BASE_API_URL}games/popular`;
export const SEARCH_GAMES_API_URL = `${BASE_API_URL}games/search?gameName={gamename}&pageSize={pagesize}`; // Append search query