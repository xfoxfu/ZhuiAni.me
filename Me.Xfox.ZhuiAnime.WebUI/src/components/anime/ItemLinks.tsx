import api, { ImportSubjectDto } from "../../api";
import { promiseWithToast } from "../../utils";
import {
  Alert,
  AlertDescription,
  AlertIcon,
  HStack,
  Icon,
  IconButton,
  Link,
  Progress,
  Tag,
  Text,
  Tooltip,
} from "@chakra-ui/react";
import React from "react";
import { IoLinkOutline, IoOpenOutline, IoRefresh, IoVideocamOutline } from "react-icons/io5";
import useSWRMutation from "swr/mutation";
import MIMEType from "whatwg-mimetype";

export const ItemLinks: React.FunctionComponent<{ id: number }> = ({ id }) => {
  const { data: links, error } = api.useItemLinkList(id);
  const bangumi =
    links &&
    links.find((x) => x.address.startsWith("https://bgm.tv/subject/") && x.mime_type === "text/html;kind=bgm.tv");
  const onRefresh = () =>
    promiseWithToast(async () => {
      if (!bangumi) {
        return;
      }
      await trigger({
        id: Number.parseInt(bangumi.address.replace("https://bgm.tv/subject/", ""), 10),
      });
    });
  // FIXME: extract this pattern to api generation
  const { trigger, isMutating } = useSWRMutation(
    "/api/modules/bangumi/import_subject",
    (_url, { arg }: { arg: ImportSubjectDto }) => api.bangumiImportSubject(arg),
    {},
  );

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
        {bangumi && (
          <Tooltip label="Refresh bgm.tv Data">
            <IconButton
              icon={<Icon as={IoRefresh} />}
              onClick={onRefresh}
              isLoading={isMutating}
              size="xs"
              aria-label="Refresh bgm.tv"
            />
          </Tooltip>
        )}
      </HStack>
    </>
  );
};
