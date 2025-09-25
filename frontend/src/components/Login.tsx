import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { setToken } from '../redux/slicers/authTokenSlice';
import { LOGIN_API_URL } from '../util/constants';

export default function Login() {
    const dispatch = useDispatch();
    const [error, setError] = useState(false);

    const loginUser = async (e: React.FormEvent) => {
        e.preventDefault();
        
        try{
            let response = await fetch(LOGIN_API_URL, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    Username: (e.target as HTMLFormElement).usernameSignIn.value,
                    Password: (e.target as HTMLFormElement).passwordSignIn.value,
                }),
            });

            if(response.ok){
                let data = await response.json();
                dispatch(setToken(data.token));
                setError(false);
            } else{
                setError(true);
            }
        }catch(err){
            console.error(err);
            setError(true);
        }
    }

    return (
        <div>
            <h2>Sign in</h2>
            <p>Sign in with your backlog account</p>
            {error && <p className="error-message">Login failed. Please try again.</p>}
            <form id="signin-form" className="form" onSubmit={e => loginUser(e)}>
                <input type="text" name="usernameSignIn" placeholder="Username" required />
                <input type="password" name="passwordSignIn" placeholder="Password" required />
                <button type="submit" className="standard-button"><span>Sign In</span></button>
            </form>
        </div>

    );
}