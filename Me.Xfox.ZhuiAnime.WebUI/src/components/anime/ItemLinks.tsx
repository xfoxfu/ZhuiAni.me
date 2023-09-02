import api from "../../api";
import { Alert, AlertDescription, AlertIcon, HStack, Icon, Link, Progress, Tag, Text, Tooltip } from "@chakra-ui/react";
import React from "react";
import { IoLinkOutline, IoOpenOutline, IoVideocamOutline } from "react-icons/io5";
import MIMEType from "whatwg-mimetype";

export const ItemLinks: React.FunctionComponent<{ id: number }> = ({ id }) => {
  const { data: links, error } = api.useItemLinkList(id);

  return (
    <>
      {error && (
        <Alert status="error">
          <AlertIcon />
          <AlertDescription>{error.message}</AlertDescription>
        </Alert>
      )}
      {!links && <Progress width="100%" size="xs" colorScheme="teal" isIndeterminate />}
      <HStack spacing={1}>
        {links &&
          links.map((i) => (
            <Link key={i.id} href={i.address} isExternal>
              <Tooltip label={i.address}>
                <Tag variant="solid" colorScheme="teal">
                  {i.mime_type === "text/html;kind=bgm.tv" ? (
                    <HStack spacing={1}>
                      <Icon as={IoOpenOutline} />
                      <Text>bgm.tv</Text>
                    </HStack>
                  ) : MIMEType.parse(i.mime_type)?.type === "video" ||
                    MIMEType.parse(i.mime_type)?.parameters.get("kind") === "text/html;kind=video" ? (
                    <Icon as={IoVideocamOutline} />
                  ) : (
                    <Icon as={IoLinkOutline} />
                  )}
                </Tag>
              </Tooltip>
            </Link>
          ))}
      </HStack>
    </>
  );
};
