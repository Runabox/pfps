import { Container, Center, Spinner, SimpleGrid, GridItem, Text, Alert, Select, FormLabel, Flex } from '@chakra-ui/react';
import Header from '../comps/header';
import Footer from '../comps/footer';
import { UploadMatchingCard, UploadSingleCard } from '../comps/uploadCard';
import { Store } from 'react-notifications-component';
import { useState, useEffect } from 'react';
import useQuery from '../comps/useQuery';
import Head from 'next/head';

interface Upload {
  id: string,
  title: string,
  description: string | null,
  tags: string[],
  uploader: string,
  isApproved: boolean,
  urls: string[],
  timestamp: string,
  views: number,
  type: number,
}

const Index = () => {
  let [isLoading, setLoading] = useState(true);
  let [uploadCards, setUploadCards] = useState<any>([]);
  let [uploadSingleCards, setUploadSingleCards] = useState<any>([]);
  let [viewCount, setViewCount] = useState(10);
  let [orderType, setOrderType] = useState(0);
  let [shownQueryAlert, setShownQueryAlert] = useState(false);
  let [loggedIn, setLoggedIn] = useState(false);
  let [madeMatchingRequest, setMadeMatchingRequest] = useState(false);
  let [madeSingleRequest, setMadeSingleRequest] = useState(false);
  let [only, setOnly] = useState(0); // 0 = none, 1 = matching, 2 = single
  let [page, setPage] = useState(0);
  let query = useQuery();

  useEffect(() => {
    if (!shownQueryAlert && !query || !localStorage) {
      return;
    }

    setLoggedIn((localStorage.getItem("token") ? true : false));

    if (!shownQueryAlert && query !== null) {
      switch (query.ref) {
        case "discord_login_error":
          Store.addNotification({
            title: "Discord Login Error",
            message: "Error logging in using discord, please try again.",
            type: "danger",
            insert: "top",
            container: "bottom-right",
            animationIn: ["animate__animated", "animate__fadeIn"],
            animationOut: ["animate__animated", "animate__fadeOut"],
            dismiss: {
              duration: 5000,
              onScreen: true
            }
          });
          break;
        case "logged_in":
          Store.addNotification({
            title: "You're already logged in!",
            message: "You are not permitted to access this resource.",
            type: "warning",
            insert: "top",
            container: "bottom-right",
            animationIn: ["animate__animated", "animate__fadeIn"],
            animationOut: ["animate__animated", "animate__fadeOut"],
            dismiss: {
              duration: 5000,
              onScreen: true
            }
          });
          break;
        case "logged_out":
          Store.addNotification({
            title: "You've been logged out.",
            message: "You've successfully been logged out of your pfps account.",
            type: "success",
            insert: "top",
            container: "bottom-right",
            animationIn: ["animate__animated", "animate__fadeIn"],
            animationOut: ["animate__animated", "animate__fadeOut"],
            dismiss: {
              duration: 5000,
              onScreen: true
            }
          });
          break;
        case "not_logged_in":
          Store.addNotification({
            title: "You're not logged in.",
            message: "You are not permitted to access this resource.",
            type: "danger",
            insert: "top",
            container: "bottom-right",
            animationIn: ["animate__animated", "animate__fadeIn"],
            animationOut: ["animate__animated", "animate__fadeOut"],
            dismiss: {
              duration: 5000,
              onScreen: true
            }
          });
          break;
        default:
          break;
      }

      setShownQueryAlert(true);
    }
  }, [query]);

  if (isLoading) {
    if (madeMatchingRequest && madeSingleRequest) {
      setLoading(false);
    }

    if (!madeMatchingRequest) {
      setMadeMatchingRequest(true);

      if (only !== 2)
        fetch(`https://api.pfps.lol/api/v1/uploads/orderby?type=${orderType}&limit=${viewCount}&page=${page}&uploadType=1`)
          .then(res => res.text())
          .then(result => {
            let uc1: any[] = [];
            if (result.length === 0) {
              uc1.push(<Text>No results found.</Text>);
              setUploadCards(uc1);
            }

            let uploads: Upload[] = JSON.parse(result);

            uploads.forEach((upload) => {
              uc1.push(
                <Center>
                  <UploadMatchingCard
                    upload={upload}
                  />
                </Center>
              );
            });

            setUploadCards(uc1);
          });
    }

    if (!madeSingleRequest) {
      setMadeSingleRequest(true);

      if (only !== 1)
        fetch(`https://api.pfps.lol/api/v1/uploads/orderby?type=${orderType}&limit=${viewCount}&page=${page}&uploadType=0`)
          .then(res => res.text())
          .then(result => {
            let uc2: any[] = [];
            if (result.length === 0) {
              uc2.push(<Text>No results found.</Text>);
              setUploadSingleCards(uc2);
            }

            let uploads: Upload[] = JSON.parse(result);
            uploads.forEach((upload) => {
              uc2.push(
                <Center>
                  <UploadSingleCard
                    upload={upload}
                  />
                </Center>
              );
            });

            setUploadSingleCards(uc2);
          });
    }



    return (
      <Container maxW="container.xl" p={0}>
        <Head>
          <script async src="https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js?client=ca-pub-5922889138174608"
            crossOrigin="anonymous"></script>
        </Head>
        <Header loggedIn={false} />

        <Center>
          <Spinner
            marginTop={100}
          />
        </Center>
      </Container >
    );
  }

  return (
    <Container maxW={['100%', 'container.xl']} p={0}>
      <Head>
        <script async src="https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js?client=ca-pub-5922889138174608"
          crossOrigin="anonymous"></script>
      </Head>
      <Header loggedIn={loggedIn} />

      <Container maxW={['100%', 'container.xl']} marginTop={30}>
        <Flex
          width={400}
        >
          <Container>
            <FormLabel><b>View</b></FormLabel>
            <Select
              value={viewCount}
              onChange={(e) => {
                setViewCount(Number.parseInt(e.target.value));
                setMadeSingleRequest(false);
                setMadeMatchingRequest(false);
                setLoading(true);
              }}
            >
              <option value={10}>10</option>
              <option value={25}>25</option>
              <option value={50}>50</option>
            </Select>
          </Container>

          <Container>
            <FormLabel><b>Sort</b></FormLabel>
            <Select
              value={orderType}
              onChange={(e) => {
                setOrderType(Number.parseInt(e.target.value));
                setMadeSingleRequest(false);
                setMadeMatchingRequest(false);
                setLoading(true);
              }}
            >
              <option value={0}>Latest</option>
              <option value={1}>Popular</option>
            </Select>
          </Container>

          <Container>
            <FormLabel><b>Show</b></FormLabel>
            <Select
              value={only}
              onChange={(e) => {
                setOnly(Number.parseInt(e.target.value));

                setUploadSingleCards([<></>]);
                setUploadCards([<></>]);

                if (Number.parseInt(e.target.value) === 1) {
                  setMadeMatchingRequest(false);
                } else if (Number.parseInt(e.target.value) === 2) {
                  setMadeSingleRequest(false);
                } else {
                  setMadeSingleRequest(false);
                  setMadeMatchingRequest(false);
                }

                setLoading(true);
              }}
            >
              <option value={0}>All</option>
              <option value={1}>Matching</option>
              <option value={2}>Single</option>
            </Select>
          </Container>
        </Flex>

        <SimpleGrid
          marginTop={5}
          columns={2}
          spacing={4}
          minChildWidth={['300px', '600px']}
        >
          {uploadCards}
        </SimpleGrid>
      </Container>

      <Container maxW={['100%', 'container.xl']} marginTop={30}>
        <hr style={{ color: 'black' }} />
        <SimpleGrid
          marginTop={5}
          spacing={4}
          minChildWidth='300px'
          columns={4}
        >
          {uploadSingleCards}
        </SimpleGrid>
      </Container>

      <Footer fixed={false} />
    </Container >
  );
}

export type {
  Upload
};

export default Index;