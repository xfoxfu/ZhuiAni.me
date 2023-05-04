import { Component } from "solid-js";

export const ImportBangumi: Component = () => {
  // const toast = useToast();
  // const [isOpen, setIsOpen] = useState(false);
  // const [isLoading, setIsLoading] = useState(false);
  // const [importLink, setImportLink] = useState("");

  // const onOpenClick = () => setIsOpen(true);
  // const onCloseClick = () => setIsOpen(false);

  // const onImportClick = () => {
  //   setIsLoading(true);
  //   Api.anime
  //     .animesImportBangumiCreate({ id: Number.parseInt(importLink, 10) })
  //     .then(() => {
  //       toast({ title: "Anime imported.", status: "success" });
  //       void Api.anime.mutateAnimesList();
  //       void Api.anime.mutateAnimesList({ include_image: true });
  //     })
  //     .catch((err) =>
  //       toast({
  //         title: "Failed to import anime.",
  //         description:
  //           err instanceof Error
  //             ? err instanceof AxiosError
  //               ? (err.response?.data as ErrorProdResponse)?.message ?? err.message
  //               : err.message
  //             : String(err),
  //         status: "error",
  //       })
  //     )
  //     .finally(() => {
  //       setIsLoading(false);
  //       setIsOpen(false);
  //     });
  // };

  return (
    <></>
    // <Box>
    //   <Popover isOpen={isOpen}>
    //     <PopoverTrigger>
    //       <Button size="sm" onClick={onOpenClick}>
    //         Import Anime from Bangumi
    //       </Button>
    //     </PopoverTrigger>
    //     <PopoverContent>
    //       <PopoverBody>
    //         <HStack>
    //           <Input placeholder="Link" value={importLink} onChange={(e) => setImportLink(e.target.value)} />
    //           <IconButton aria-label="Import" icon={<CheckIcon />} onClick={onImportClick} isLoading={isLoading} />
    //           <IconButton aria-label="Cancel" icon={<CloseIcon />} onClick={onCloseClick} isDisabled={isLoading} />
    //         </HStack>
    //       </PopoverBody>
    //     </PopoverContent>
    //   </Popover>
    // </Box>
  );
};
