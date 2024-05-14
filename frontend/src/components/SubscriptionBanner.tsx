import { Flex, Spacer, Text, useMediaQuery, Icon } from '@chakra-ui/react';
import { FaTools, FaHandshake } from 'react-icons/fa';
 
export default function SubscriptionPage() {
  const [isLargerThan48] = useMediaQuery('(min-width: 4em)');
 
  const array = [
    {
      id: 1,
      text: ' Solving world ... [truncated]',
      icon: FaTools,
    },
    {
      id: 2,
      text: 'Through team work, ... [truncated]',
      icon: FaHandshake,
    }
  ];
 
  return (
    <Flex
      display="flex"
      minH="80vh"
      alignItems="center"
      justifyContent="center"
      flexWrap="wrap"
      flexDirection={isLargerThan48 ? 'row' : 'column'}
      px={isLargerThan48 ? '16' : '6'}
      border="10px solid #e3e3e3"
    >
      {array.map((arr) => (
        <>
          <Flex
            minH="70vh"
            bg="#fff"
            width={isLargerThan48 ? '25%' : 'full'}
            shadow="md"
            p="6"
            alignItems="center"
            justifyContent="center"
            borderRadius="7"
            flexDirection="column"
            textAlign="center"
            mb={isLargerThan48 ? '0' : '4'}
            border="1px solid #e3e3e3"

          >
            <Icon as={arr.icon} boxSize={14} color="blue.600" mb="5" />
            <Text
              bgGradient='linear(to-l, #7928CA, #FF0080)'
              bgClip='text'
              fontSize='6xl'
              fontWeight='extrabold'
             >{arr.text}
            </Text>
          </Flex>
 
          <Spacer />
        </>
      ))}
    </Flex>
  );
};