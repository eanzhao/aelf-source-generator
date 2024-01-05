using AElf.Contracts.MultiToken;
using AElf.Sdk.CSharp;
using AElf.Standards.ACS1;
using AElf.Standards.ACS2;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Contracts.Test;

public class TokenContract : TokenContractImplContainer.TokenContractImplBase
{
    public override TransactionFeeDelegations GetTransactionFeeDelegateInfo(GetTransactionFeeDelegateInfoInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty SetMethodFee(MethodFees input)
    {
        throw new NotImplementedException();
    }

    public override Empty ChangeMethodFeeController(AuthorityInfo input)
    {
        throw new NotImplementedException();
    }

    public override MethodFees GetMethodFee(StringValue input)
    {
        throw new NotImplementedException();
    }

    public override AuthorityInfo GetMethodFeeController(Empty input)
    {
        throw new NotImplementedException();
    }

    public override ResourceInfo GetResourceInfo(Transaction input)
    {
        throw new NotImplementedException();
    }

    public override Empty Create(CreateInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty Issue(IssueInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty Transfer(TransferInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty TransferFrom(TransferFromInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty Approve(ApproveInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty UnApprove(UnApproveInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty Lock(LockInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty Unlock(UnlockInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty Burn(BurnInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty SetPrimaryTokenSymbol(SetPrimaryTokenSymbolInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty CrossChainTransfer(CrossChainTransferInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty CrossChainReceiveToken(CrossChainReceiveTokenInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty CrossChainCreateToken(CrossChainCreateTokenInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty InitializeFromParentChain(InitializeFromParentChainInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty ClaimTransactionFees(TotalTransactionFeesMap input)
    {
        throw new NotImplementedException();
    }

    public override ChargeTransactionFeesOutput ChargeTransactionFees(ChargeTransactionFeesInput input)
    {
        throw new NotImplementedException();
    }

    public override ChargeTransactionFeesOutput ChargeUserContractTransactionFees(ChargeTransactionFeesInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty CheckThreshold(CheckThresholdInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty InitialCoefficients(Empty input)
    {
        throw new NotImplementedException();
    }

    public override Empty DonateResourceToken(TotalResourceTokensMaps input)
    {
        throw new NotImplementedException();
    }

    public override Empty ChargeResourceToken(ChargeResourceTokenInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty CheckResourceToken(Empty input)
    {
        throw new NotImplementedException();
    }

    public override Empty SetSymbolsToPayTxSizeFee(SymbolListToPayTxSizeFee input)
    {
        throw new NotImplementedException();
    }

    public override Empty UpdateCoefficientsForSender(UpdateCoefficientsInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty UpdateCoefficientsForContract(UpdateCoefficientsInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty InitializeAuthorizedController(Empty input)
    {
        throw new NotImplementedException();
    }

    public override Empty AddAddressToCreateTokenWhiteList(Address input)
    {
        throw new NotImplementedException();
    }

    public override Empty RemoveAddressFromCreateTokenWhiteList(Address input)
    {
        throw new NotImplementedException();
    }

    public override SetTransactionFeeDelegationsOutput SetTransactionFeeDelegations(SetTransactionFeeDelegationsInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty RemoveTransactionFeeDelegator(RemoveTransactionFeeDelegatorInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty RemoveTransactionFeeDelegatee(RemoveTransactionFeeDelegateeInput input)
    {
        throw new NotImplementedException();
    }

    public override GetTransactionFeeDelegateesOutput GetTransactionFeeDelegatees(GetTransactionFeeDelegateesInput input)
    {
        throw new NotImplementedException();
    }

    public override TokenInfo GetTokenInfo(GetTokenInfoInput input)
    {
        throw new NotImplementedException();
    }

    public override TokenInfo GetNativeTokenInfo(Empty input)
    {
        throw new NotImplementedException();
    }

    public override TokenInfoList GetResourceTokenInfo(Empty input)
    {
        throw new NotImplementedException();
    }

    public override GetBalanceOutput GetBalance(GetBalanceInput input)
    {
        throw new NotImplementedException();
    }

    public override GetAllowanceOutput GetAllowance(GetAllowanceInput input)
    {
        throw new NotImplementedException();
    }

    public override BoolValue IsInWhiteList(IsInWhiteListInput input)
    {
        throw new NotImplementedException();
    }

    public override GetLockedAmountOutput GetLockedAmount(GetLockedAmountInput input)
    {
        throw new NotImplementedException();
    }

    public override Address GetCrossChainTransferTokenContractAddress(GetCrossChainTransferTokenContractAddressInput input)
    {
        throw new NotImplementedException();
    }

    public override StringValue GetPrimaryTokenSymbol(Empty input)
    {
        throw new NotImplementedException();
    }

    public override CalculateFeeCoefficients GetCalculateFeeCoefficientsForContract(Int32Value input)
    {
        throw new NotImplementedException();
    }

    public override CalculateFeeCoefficients GetCalculateFeeCoefficientsForSender(Empty input)
    {
        throw new NotImplementedException();
    }

    public override SymbolListToPayTxSizeFee GetSymbolsToPayTxSizeFee(Empty input)
    {
        throw new NotImplementedException();
    }

    public override Hash GetLatestTotalTransactionFeesMapHash(Empty input)
    {
        throw new NotImplementedException();
    }

    public override Hash GetLatestTotalResourceTokensMapsHash(Empty input)
    {
        throw new NotImplementedException();
    }

    public override BoolValue IsTokenAvailableForMethodFee(StringValue input)
    {
        throw new NotImplementedException();
    }

    public override StringList GetReservedExternalInfoKeyList(Empty input)
    {
        throw new NotImplementedException();
    }

    public override TransactionFeeDelegations GetTransactionFeeDelegationsOfADelegatee(GetTransactionFeeDelegationsOfADelegateeInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty AdvanceResourceToken(AdvanceResourceTokenInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty TakeResourceTokenBack(TakeResourceTokenBackInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty RegisterCrossChainTokenContractAddress(RegisterCrossChainTokenContractAddressInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty SetFeeReceiver(Address input)
    {
        throw new NotImplementedException();
    }

    public override Empty ValidateTokenInfoExists(ValidateTokenInfoExistsInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty UpdateRental(UpdateRentalInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty UpdateRentedResources(UpdateRentedResourcesInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty TransferToContract(TransferToContractInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty ChangeSideChainRentalController(AuthorityInfo input)
    {
        throw new NotImplementedException();
    }

    public override Empty ChangeSymbolsToPayTXSizeFeeController(AuthorityInfo input)
    {
        throw new NotImplementedException();
    }

    public override Empty ChangeCrossChainTokenContractRegistrationController(AuthorityInfo input)
    {
        throw new NotImplementedException();
    }

    public override Empty ChangeUserFeeController(AuthorityInfo input)
    {
        throw new NotImplementedException();
    }

    public override Empty ChangeDeveloperController(AuthorityInfo input)
    {
        throw new NotImplementedException();
    }

    public override Empty ConfigTransactionFeeFreeAllowances(ConfigTransactionFeeFreeAllowancesInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty RemoveTransactionFeeFreeAllowancesConfig(RemoveTransactionFeeFreeAllowancesConfigInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty SetTransactionFeeDelegateInfos(SetTransactionFeeDelegateInfosInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty RemoveTransactionFeeDelegatorInfos(RemoveTransactionFeeDelegatorInfosInput input)
    {
        throw new NotImplementedException();
    }

    public override Empty RemoveTransactionFeeDelegateeInfos(RemoveTransactionFeeDelegateeInfosInput input)
    {
        throw new NotImplementedException();
    }

    public override Address GetFeeReceiver(Empty input)
    {
        throw new NotImplementedException();
    }

    public override ResourceUsage GetResourceUsage(Empty input)
    {
        throw new NotImplementedException();
    }

    public override AuthorityInfo GetSymbolsToPayTXSizeFeeController(Empty input)
    {
        throw new NotImplementedException();
    }

    public override AuthorityInfo GetCrossChainTokenContractRegistrationController(Empty input)
    {
        throw new NotImplementedException();
    }

    public override UserFeeController GetUserFeeController(Empty input)
    {
        throw new NotImplementedException();
    }

    public override DeveloperFeeController GetDeveloperFeeController(Empty input)
    {
        throw new NotImplementedException();
    }

    public override AuthorityInfo GetSideChainRentalControllerCreateInfo(Empty input)
    {
        throw new NotImplementedException();
    }

    public override Address GetVirtualAddressForLocking(GetVirtualAddressForLockingInput input)
    {
        throw new NotImplementedException();
    }

    public override OwningRental GetOwningRental(Empty input)
    {
        throw new NotImplementedException();
    }

    public override OwningRentalUnitValue GetOwningRentalUnitValue(Empty input)
    {
        throw new NotImplementedException();
    }

    public override TransactionFeeFreeAllowancesMap GetTransactionFeeFreeAllowances(Address input)
    {
        throw new NotImplementedException();
    }

    public override GetTransactionFeeFreeAllowancesConfigOutput GetTransactionFeeFreeAllowancesConfig(Empty input)
    {
        throw new NotImplementedException();
    }

    public override GetTransactionFeeDelegateeListOutput GetTransactionFeeDelegateeList(GetTransactionFeeDelegateeListInput input)
    {
        throw new NotImplementedException();
    }
}