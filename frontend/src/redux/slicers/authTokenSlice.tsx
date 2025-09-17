import { createSlice } from "@reduxjs/toolkit";

const initialState = {
    token: null as string | null,
    isAuthenticated: false,
}

const authTokenSlice = createSlice({
    name: 'authToken',
    initialState,
    reducers: {
        setToken: (state, action) => { 
            state.token = action.payload;
            state.isAuthenticated = true;
        },
        clearToken: (state) => {
            state.token = null;
            state.isAuthenticated = false;
        }
    }
})

export const { setToken, clearToken } = authTokenSlice.actions;

export default authTokenSlice.reducer;