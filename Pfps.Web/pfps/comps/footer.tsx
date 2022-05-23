import { Text, Center } from '@chakra-ui/react';

const Footer = ({ fixed, top }: { fixed: boolean; top?: number }) => {
    if (fixed) {
        return (
            <Center
                position="fixed"
                left={0}
                right={0}
                bottom={2}
            >
                <Text color="gray">
                    © 2022 Runa Holdings LLC | <a href="mailto:dmca@runa.live"><u>DMCA</u></a> | <a href="/privacy.html" target="_blank"><u>Privacy Policy</u></a> | <a href="/tos.html" target="_blank"><u>Terms of Service</u></a> | <a href="https://discord.gg/wSQ6zSc2mB" target="_blank"><u>Discord</u></a>
                </Text>
            </Center>
        );
    }
    return (
        <Center
            marginTop={top ? top : 200}
            marginBottom={2}
        >
            <Text color="gray">
                © 2022 Runa Holdings LLC | <a href="mailto:dmca@runa.live"><u>DMCA</u></a> | <a href="/privacy.html" target="_blank"><u>Privacy Policy</u></a> | <a href="/tos.html" target="_blank"><u>Terms of Service</u></a> | <a href="https://discord.gg/wSQ6zSc2mB" target="_blank"><u>Discord</u></a>
            </Text>
        </Center>
    );
}

export default Footer;