import { Container, Flex, Box, Stack, Text, Button, Heading, Badge, IconButton, Menu, MenuList, MenuItem, Tooltip, MenuButton, MenuDivider, Center, useColorMode, CloseButton, Avatar, AvatarBadge } from '@chakra-ui/react';
import { HamburgerIcon, BellIcon, MoonIcon, SunIcon, CloseIcon } from '@chakra-ui/icons';
import { useState, useEffect } from 'react';

const Header = ({ loggedIn }: { loggedIn: boolean }) => {
    let [isOpen, setIsOpen] = useState(false);
    const { colorMode, toggleColorMode } = useColorMode();

    return (
        <Center>
            <Flex
                as="nav"
                align="center"
                justify="space-between"
                wrap="wrap"
                padding={5}
                bg="purple.500"
                color="white"
                marginTop={5}
                width={['95%', '95%', '100%']}
                borderRadius="xl"
            >
                <Flex align="center" mr={5}>
                    <Heading as="h1" size="lg" letterSpacing={"tighter"} _hover={{ opacity: '75%' }}>
                        <a href="/">pfps</a>
                    </Heading>
                    <Badge marginLeft={2} marginTop={2} bg="purple.300">alpha</Badge>
                </Flex>

                <Box display={{ base: "block", md: "none" }}>
                    <IconButton
                        onClick={toggleColorMode}
                        aria-label='Theme toggle'
                        icon={(colorMode === 'light' ? <MoonIcon /> : <SunIcon />)}
                        bg="purple.700"
                        _hover={{ bg: "purple.300", borderColor: "purple.300" }}
                        marginRight={2}
                    />
                    <Menu>
                        <MenuButton
                            as={IconButton}
                            aria-label='Navbar'
                            icon={<HamburgerIcon />}
                            variant='outline'
                            _hover={{ bg: 'purple.700', borderColor: 'purple.700' }}
                            _active={{ bg: 'purple.700', borderColor: 'purple.700' }}
                        />
                        <MenuList
                        >
                            {loggedIn ?
                                <>
                                    <MenuItem
                                        as="a"
                                        href="/account/posts"
                                    >
                                        Your Posts
                                    </MenuItem>
                                    <MenuItem
                                        as="a"
                                        href="/account/favorites"
                                    >
                                        Favorites
                                    </MenuItem>
                                    <MenuDivider />
                                    <MenuItem
                                        icon={<BellIcon />}
                                    >
                                        Alerts
                                    </MenuItem>
                                    <MenuItem
                                        as="a"
                                        href="/upload"
                                    >
                                        Upload
                                    </MenuItem>
                                    <MenuItem
                                        as="a"
                                        href="/account"
                                    >
                                        Account
                                    </MenuItem></> :
                                <>
                                    <MenuItem
                                        as="a"
                                        href="/login"
                                    >
                                        Login
                                    </MenuItem>
                                </>}
                        </MenuList>
                    </Menu>
                </Box>

                <Stack
                    direction={{ base: "column", md: "row" }}
                    display={{ base: isOpen ? "block" : "none", md: "flex" }}
                    width={{ base: "full", md: "auto" }}
                    alignItems="center"
                    flexGrow={1}
                    mt={{ base: 4, md: 0 }}
                >
                    {loggedIn ? <LoggedInLinks /> : <></>}
                </Stack>

                <Box
                    display={{ base: isOpen ? "block" : "none", md: "block" }}
                    mt={{ base: 4, md: 0 }}
                >
                    <Tooltip hasArrow placement="top" label="Toggle theme">
                        <IconButton
                            onClick={toggleColorMode}
                            aria-label='Theme toggle'
                            icon={(colorMode === 'light' ? <MoonIcon /> : <SunIcon />)}
                            bg="purple.700"
                            _hover={{ bg: "purple.300", borderColor: "purple.300" }}
                            marginRight={2}
                        />
                    </Tooltip>
                    {loggedIn ? <LoggedInButtons token={process.browser ? localStorage.getItem("token") : null} /> : <LoggedOutButtons />}
                </Box>
            </Flex>
        </Center>
    );
};

const LoggedInLinks = () => {
    return (
        <>
            <a href="/account/posts">
                <Text
                    marginLeft={3}
                    marginTop={1}
                    _hover={{ color: "purple.700" }}
                >
                    Your Posts
                </Text>
            </a>

            <a href="/account/favorites">
                <Text
                    marginLeft={3}
                    marginTop={1}
                    _hover={{ color: "purple.700" }}
                >
                    Favorites
                </Text>
            </a>
        </>
    );
};

const LoggedInButtons = ({ token }: { token: string | null }) => {
    if (token === null) {
        return (<Text>Error retrieving token.</Text>);
    }

    let [notifications, setNotifications] = useState([<></>]);
    let [madeNotiRequest, setMadeNotiRequest] = useState(false);
    let [notiCount, setNotiCount] = useState(0);
    let [loading, setLoading] = useState(true);

    if (loading) {
        if (!madeNotiRequest) {
            setMadeNotiRequest(true);

            fetch('https://api.pfps.lol/api/v1/users/@me/notifications', {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            })
                .then(res => res.json())
                .then((notificationss: any[]) => {
                    setNotiCount(notificationss.length);
                    notificationss.reverse();

                    let notis = [<></>];
                    notificationss.forEach((notification, index) => {
                        let notificationBody = [<></>];
                        if (notification.type === 0) {
                            notificationBody.push(<>Your post <i>{notification.upload.title}</i> has been denied approval by <b>{notification.moderator.username}</b> for the following reason{'(s)'}:<br /><b><i>{notification.message}</i></b></>);
                        } else if (notification.type === 1) {
                            notificationBody.push(<>Your post <i>{notification.upload.title}</i> has been approved by <b>{notification.moderator.username}</b>.</>);
                        } else if (notification.type == 2) {
                            notificationBody.push(<>Your post <i>{notification.upload.title}</i> has been deleted by <b>{notification.moderator.username}</b> for the following reason{'(s)'}:<br /><b><i>{notification.message}</i></b></>);
                        } else {
                            notificationBody.push(<>Unknown notification type.</>);
                        }

                        notis.push(
                            <MenuItem
                                as={Container}
                            >
                                <Flex>
                                    <Text width={300}>{notificationBody}</Text>
                                    <>
                                        <Tooltip placement="top" label="Dismiss" hasArrow>
                                            <IconButton
                                                aria-label='Dismiss'
                                                icon={<CloseIcon />}
                                                onClick={async (e) => {
                                                    e.preventDefault();

                                                    // send dismiss request and remove self
                                                    let res = await fetch(`https://api.pfps.lol/api/v1/notifications/${notification.id}`, {
                                                        method: 'delete',
                                                        headers: {
                                                            Authorization: `Bearer ${token}`,
                                                        },
                                                    });

                                                    if (res.status !== 204) {
                                                        // Show error alert
                                                        return;
                                                    }

                                                    setMadeNotiRequest(false);
                                                    setLoading(true);
                                                }}
                                            />
                                        </Tooltip>
                                    </>
                                </Flex>
                            </MenuItem>
                        );
                        notis.push(<MenuDivider />);
                    });

                    setNotifications(notis);
                    setLoading(false);
                });
        }

        return (<></>);
    }

    return (
        <>
            <Menu>
                <Tooltip hasArrow label="Notifications" placement="top">
                    <MenuButton
                        as={Button}
                        color='white'
                        alignItems='center'
                        aria-label='Alerts'
                        icon={<Center><BellIcon /></Center>}
                        bg="purple.700"
                        _hover={{ bg: "purple.300", borderColor: "purple.300" }}
                        marginRight={2}
                        borderRadius='6px'
                        width='40px'
                        height='40px'
                        position="relative"
                    >
                        <Center>
                            <BellIcon />
                        </Center>

                        {notiCount >= 1 ?
                            <AvatarBadge boxSize="1.25em" bg="red.500" borderRadius=".75em" position="absolute" left={6} top={6}>
                                {notiCount >= 10 ? <Text fontSize={14}>9+</Text> : notiCount}
                            </AvatarBadge>
                            : <></>}
                    </MenuButton>
                </Tooltip>
                <Button
                    variant="outline"
                    _hover={{ bg: "purple.700", borderColor: "purple.700" }}
                    marginRight={2}
                    as="a"
                    href="/upload"
                >
                    Upload
                </Button>
                <Button
                    variant="outline"
                    _hover={{ bg: "purple.700", borderColor: "purple.700" }}
                    as="a"
                    href="/account"
                >
                    Account
                </Button>
                <MenuList
                    maxHeight={500}
                    overflowY='auto'
                >
                    <Text marginLeft={3} marginTop={1} fontSize='170%'><b>Notifications</b></Text>
                    <MenuDivider />
                    {notifications.length <= 1 ? <Text marginLeft={3}><i>No results found.</i></Text> : notifications}
                </MenuList>
            </Menu>

        </>
    );
}

const LoggedOutButtons = () => {
    return (
        <Button
            variant="outline"
            _hover={{ bg: "purple.700", borderColor: "purple.700" }}
            as="a"
            href="/login"
        >
            Login
        </Button>
    );
};

export default Header;