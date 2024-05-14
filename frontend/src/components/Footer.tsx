import { Flex, Text, Link } from '@chakra-ui/react';
 
export default function Footer() {
  return (
    <Flex
      w="full"
      bg="blackAlpha.50"
      minHeight="20vh"
      flexDirection="column"
      alignItems="center"
      textAlign="center"
      justifyContent="center"
    >
      <Text mb="3">
        Provided by{' '}
        <Link href="https://appseed.us" isExternal color="blue.500">
          AppSeed
        </Link>
      </Text>
      <Text opacity="0.5">Open-Source Sample - Buit with Chakra UI</Text>
    </Flex>
  );
};