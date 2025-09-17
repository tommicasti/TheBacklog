import { configureStore } from '@reduxjs/toolkit';
import authTokenReducer from './slicers/authTokenSlice';

export const store = configureStore({
    reducer: {
        authToken: authTokenReducer,
    },
});