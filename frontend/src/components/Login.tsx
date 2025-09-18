import React from 'react';
import { useDispatch } from 'react-redux';
import { setToken } from '../redux/slicers/authTokenSlice';
import { LOGIN_API_URL } from '../util/constants';

export default function Login() {
    const dispatch = useDispatch();

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
                console.log(data);
                dispatch(setToken(data.token));
            }
        }catch(err){
            console.error(err);
        }
    }

    return (
        <div>
            <h2>Sign in</h2>
            <p>Sign in with your backlog account</p>
            <form id="signin-form" className="form" onSubmit={e => loginUser(e)}>
                <input type="text" name="usernameSignIn" placeholder="Username" required />
                <input type="password" name="passwordSignIn" placeholder="Password" required />
                <button type="submit" className="standard-button"><span>Sign In</span></button>
            </form>
        </div>

    );
}