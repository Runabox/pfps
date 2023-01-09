import { AppProps } from 'next/app';
import { ChakraProvider, ColorModeScript } from '@chakra-ui/react';
import { useEffect } from 'react';
import { ReactNotifications } from 'react-notifications-component';
import 'react-notifications-component/dist/theme.css';
import Head from 'next/head';
import theme from '../comps/theme';
import { GoogleAnalytics, usePagesViews } from 'nextjs-google-analytics';
import Script from 'next/script';

const App = ({ Component, pageProps }: AppProps) => {
  usePagesViews();

  useEffect(() => {
    if (!localStorage.getItem("shownMobileAlert")) {
      const ua = navigator.userAgent;
      if (/(tablet|ipad|playbook|silk)|(android(?!.*mobi))/i.test(ua)) {
        alert('WARNING: This website is not made to run on mobile devices. It may be unusable.');
      }
      else if (/Mobile|Android|iP(hone|od)|IEMobile|BlackBerry|Kindle|Silk-Accelerated|(hpw|web)OS|Opera M(obi|ini)/.test(ua)) {
        alert('WARNING: This website is not made to run on mobile devices. It may be unusable.');
      }

      localStorage.setItem("shownMobileAlert", "true");
    }

    if (localStorage.getItem("token")) {
      fetch('https://api.pfps.lol/api/v1/users/@me', {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`
        }
      })
        .then(res => res.json())
        .catch(e => {
          localStorage.setItem("token", "");
          window.location.href = '/?ref=logged_out';
        });
    }
  }, []);

  return (
    <>
      <Head>
        <title>pfps.lol alpha</title>

        <Script
          src='https://www.google.com/recaptcha/api.js?&render=explicit'
          async
          defer
        />
      </Head>

      <GoogleAnalytics
        gaMeasurementId='G-VY73X6Q3M4'
      />
      <ColorModeScript initialColorMode={theme.config.initialColorMode} />
      <ChakraProvider>
        <ReactNotifications />
        <Component {...pageProps} />
      </ChakraProvider>
    </>
  );
}

export default App;
