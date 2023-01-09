import { Container, Center, Spinner, Text, Select, FormLabel, SimpleGrid, IconButton, Tooltip, FormControl, Input, Button, Flex, Tag, TagCloseButton, HStack, TagLabel, Box } from '@chakra-ui/react';

import Header from '../comps/header';
import Footer from '../comps/footer';

import { useState, useEffect, SetStateAction, Dispatch, useCallback, SyntheticEvent } from 'react';
import ChakraTagInput from '../comps/ChakraTagInput';
import Captcha from '../comps/captcha';

const Upload = () => {
    const [loading, setLoading] = useState<boolean>(true);
    const [type, setType] = useState<number>(1);
    const [title, setTitle] = useState<string>("");
    const [description, setDescription] = useState<string>("");
    const [tagStrings, setTagStrings] = useState<string[]>();
    const [file1, setFile1] = useState<File | string>("");
    const [file2, setFile2] = useState<File | string>("");
    const [reCAPTCHA, setReCAPTCHA] = useState("");

    useEffect(() => {
        if (!localStorage.getItem("token")) {
            window.location.href = '/?ref=not_logged_in';
            return;
        }

        setLoading(false);
    }, []);

    const handleTagsChange = useCallback((event: SyntheticEvent, tags: string[]) => {
        setTagStrings(tags);
    }, []);

    if (loading) {
        return (
            <Container maxW="container.xl">
                <Header loggedIn={true} />

                <Center>
                    <Spinner marginTop={100} />
                </Center>

                <Footer fixed={true} />
            </Container>
        );
    }

    return (
        <Container maxW="container.xl">
            <Header loggedIn={true} />

            <Container
                padding={5}
                maxW="container.xl"
            >
                <Center>
                    <Box
                        padding={10}
                        width={600}
                        height={850}
                        borderRadius="15px"
                        boxShadow="em"
                        borderWidth="1px"
                    >
                        <Text fontSize="250%">Create Post</Text>
                        <FormControl isRequired mt={6}>
                            <FormLabel>Post Type</FormLabel>
                            <Select width="50%" value={type === 1 ? "matching" : "single"} onChange={e => (e.target.value === "matching" ? setType(1) : setType(0))}>
                                <option value='single'>One Profile Picture</option>
                                <option value='matching'>Matching Profile Pictures</option>
                            </Select>
                        </FormControl>
                        <FormControl isRequired mt={6}>
                            <FormLabel>Title</FormLabel>
                            <Input
                                type="text"
                                placeholder="Title"
                                onChange={event => setTitle(event.currentTarget.value)}
                                maxLength={48}
                            />
                        </FormControl>
                        <FormControl mt={6}>
                            <FormLabel>Description</FormLabel>
                            <Input
                                type="text"
                                placeholder="Description"
                                onChange={event => setDescription(event.currentTarget.value)}
                                maxLength={128}
                            />
                        </FormControl>
                        <FormControl isRequired mt={6}>
                            <FormLabel>Tags</FormLabel>
                            <ChakraTagInput
                                tags={tagStrings}
                                onTagsChange={handleTagsChange}
                            />
                        </FormControl>
                        <br />

                        <br />
                        {type === 0 ? <SingleUploadButtons changeEvent1={setFile1} /> :
                            <MatchingUploadButtons changeEvent1={setFile1} changeEvent2={setFile2} />}

                        <br />
                        <br />

                        <Center
                            marginTop={3}
                        >
                            <Captcha
                                response={(res: string) => {
                                    setReCAPTCHA(res);
                                }}
                            />
                        </Center>

                        <br />

                        <Center>
                            <Button
                                bg="purple.500"
                                color="white"
                                width="70%"
                                _hover={{ bg: "purple.300" }}
                                onClick={async () => {
                                    if (title === null || title === undefined || title === "" || title === " " || title.includes("ㅤ")) {
                                        return alert('Title is empty');
                                    }

                                    if (file1 === "" || (file2 === "" && type === 1)) {
                                        return alert('File is empty');
                                    }

                                    if (tagStrings!.length !== 0 && tagStrings!.length < 2) {
                                        return alert('Minimum of 2 tags.');
                                    }

                                    let data = new FormData();
                                    data.append("upload", file1);
                                    if (file2 !== "") {
                                        data.append("upload", file2);
                                    }
                                    data.append("title", title);
                                    if (description !== "") {
                                        data.append("description", description);
                                    }
                                    data.append("type", `${type}`);

                                    if (tagStrings !== undefined && tagStrings.length !== 0) {
                                        let tagStringFinal = "";
                                        tagStrings.forEach((tag) => {
                                            tagStringFinal = `${tagStringFinal}${tag},`;
                                        });

                                        tagStringFinal = tagStringFinal.slice(0, tagStringFinal.length - 1);

                                        data.append("tags", tagStringFinal);
                                    }

                                    let res = await fetch('https://api.pfps.lol/api/v1/upload', {
                                        body: data,
                                        method: 'post',
                                        headers: {
                                            Authorization: `Bearer ${localStorage.getItem("token")}`,
                                            'recaptcha-repsonse': reCAPTCHA
                                        },
                                    });

                                    let result = await res.json();
                                    if (res.status !== 200) {
                                        alert(`Error uploading post - ${JSON.stringify(result.error)}`);
                                        return window.location.reload();
                                    }

                                    alert("Successfully uploaded! Please allow up to 24 hours for content to be approved.");
                                    window.location.reload();
                                }}
                            >
                                Upload
                            </Button>
                        </Center>
                    </Box>
                </Center>
            </Container >

            <Footer fixed={false} top={50} />
        </Container >
    );
};

const SingleUploadButtons = (props: { changeEvent1: Dispatch<SetStateAction<File | string>> }) => {
    return (
        <Tooltip hasArrow label="Upload Image" placement="top">
            <input
                type="file"
                aria-label='Upload Image'
                width={250}
                height={250}
                onChange={(e) => props.changeEvent1(e.currentTarget.files![0])}
            />
        </Tooltip>
    );
}

const MatchingUploadButtons = (props: { changeEvent1: Dispatch<SetStateAction<File | string>>, changeEvent2: Dispatch<SetStateAction<File | string>> }) => {
    return (
        <SimpleGrid
            columns={2}
            spacing={6}
        >
            <FormLabel>Left</FormLabel>
            <Tooltip hasArrow label="Upload Image" placement="top">
                <input
                    type="file"
                    aria-label='Upload Image'
                    width={250}
                    height={250}
                    onChange={(e) => props.changeEvent1(e.currentTarget.files![0])}
                />
            </Tooltip>

            <FormLabel>Right</FormLabel>
            <Tooltip hasArrow label="Upload Image" placement="top">
                <input
                    type="file"
                    aria-label='Upload Image'
                    width={250}
                    height={250}
                    onChange={(e) => props.changeEvent2(e.currentTarget.files![0])}
                />
            </Tooltip>
        </SimpleGrid>
    );
}

export default Upload;