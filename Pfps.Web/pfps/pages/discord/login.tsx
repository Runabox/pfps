import useQuery from '../../comps/useQuery';
import { useEffect } from 'react';

const DiscordLogin = () => {
    const query = useQuery();

    useEffect(() => {
        if (!query) {
            return;
        }

        fetch('https://api.pfps.lol/api/v1/discord/login?code=' + query.code)
            .then(res => res.json())
            .then(result => {
                if (!result.token) {
                    window.location.href = '/?ref=discord_login_error';
                    return;
                }

                localStorage.setItem("token", result.token);
                window.location.href = '/';
            });
    }, [query]);

    return <></>;
}

export default DiscordLogin;