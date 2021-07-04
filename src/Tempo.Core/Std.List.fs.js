import { sortBy, map, append, length, fold, empty, cons, foldBack } from "../Tempo.Demo/.fable/fable-library.3.1.10/List.js";
import { value } from "../Tempo.Demo/.fable/fable-library.3.1.10/Option.js";
import { Union } from "../Tempo.Demo/.fable/fable-library.3.1.10/Types.js";
import { union_type } from "../Tempo.Demo/.fable/fable-library.3.1.10/Reflection.js";
import { List_groupBy } from "../Tempo.Demo/.fable/fable-library.3.1.10/Seq2.js";
import { compare, structuralHash, equals } from "../Tempo.Demo/.fable/fable-library.3.1.10/Util.js";

export function filterMap(f, ls) {
    return foldBack((curr, acc) => {
        const matchValue = f(curr);
        return (matchValue == null) ? acc : cons(value(matchValue), acc);
    }, ls, empty());
}

export class RankStrategy extends Union {
    constructor(tag, ...fields) {
        super();
        this.tag = (tag | 0);
        this.fields = fields;
    }
    cases() {
        return ["StandardCompetition", "ModifiedCompetition", "DenseRanking"];
    }
}

export function RankStrategy$reflection() {
    return union_type("Tempo.Std.List.RankStrategy", [], RankStrategy, () => [[], [], []]);
}

export function rank(getScore, strategy, ls) {
    return fold((tupledArg_2, rest) => {
        let patternInput;
        const groupLength = length(rest) | 0;
        const current = tupledArg_2[0] | 0;
        switch (strategy.tag) {
            case 1: {
                patternInput = [current + groupLength, 0];
                break;
            }
            case 2: {
                patternInput = [current + 1, 0];
                break;
            }
            default: {
                patternInput = [current + 1, groupLength - 1];
            }
        }
        const assign_1 = patternInput[0] | 0;
        return [assign_1 + patternInput[1], append(tupledArg_2[1], map((v_1) => [assign_1, v_1], rest))];
    }, [0, empty()], map((tupledArg_1) => tupledArg_1[1], sortBy((tupledArg) => tupledArg[0], List_groupBy(getScore, ls, {
        Equals: (x, y) => equals(x, y),
        GetHashCode: (x) => structuralHash(x),
    }), {
        Compare: (x_1, y_1) => compare(x_1, y_1),
    })))[1];
}

